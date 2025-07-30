using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Common.Models.InvoiceModels;
using Eventify.Application.Common.Models.PaymentModels;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Specifications.BookingSpecifications;
using Eventify.Domain.Specifications.PaymentSpecifications;
using Eventify.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using QuestPDF.Helpers;

namespace Eventify.Infrastructure.Services
{
    public class InvoiceService : IInvoiceService
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IEmailService _emailService;
        private readonly ILogger<InvoiceService> _logger;

        public InvoiceService(
            IRepository<Booking> bookingRepository,
            IRepository<Payment> paymentRepository,
            IEmailService emailService,
            ILogger<InvoiceService> logger)
        {
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result<InvoiceDocument>> GenerateInvoiceAsync(Guid bookingId)
        {
            try
            {
                var bookingSpec = new BookingWithDetailsSpecification(bookingId);
                var booking = await _bookingRepository.GetBySpecAsync(bookingSpec);

                if (booking == null)
                {
                    return Result<InvoiceDocument>.Failure("Booking not found");
                }

                var invoice = new InvoiceDocument
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = GenerateInvoiceNumber(),
                    BookingId = booking.Id,
                    CustomerName = booking.User.FullName,
                    CustomerEmail = booking.User.Email,
                    InvoiceDate = DateTime.UtcNow,
                    DueDate = DateTime.UtcNow.AddDays(30),
                    Status = InvoiceStatus.Draft,
                    Currency = booking.TotalAmount.Currency,
                    CreatedAt = DateTime.UtcNow
                };

                // Calculate line items
                var lineItems = new List<InvoiceLineItem>();
                foreach (var ticket in booking.Tickets)
                {
                    lineItems.Add(new InvoiceLineItem
                    {
                        Description = $"{ticket.TicketType.Name} - {booking.Event.Title}",
                        Quantity = 1,
                        UnitPrice = ticket.Price,
                        TotalPrice = ticket.Price
                    });
                }

                invoice.LineItems = lineItems;
                invoice.Subtotal = new Money(lineItems.Sum(li => li.TotalPrice.Amount), invoice.Currency);

                // Calculate tax if applicable
                var taxRate = 0.1m; // 10% this should be configurable
                invoice.TaxAmount = new Money(invoice.Subtotal.Amount * taxRate, invoice.Currency);
                invoice.Total = invoice.Subtotal.Add(invoice.TaxAmount);

                // Set billing address from user
                if (booking.User.Address != null)
                {
                    invoice.BillingAddress = new BillingDetails
                    {
                        Name = booking.User.FullName,
                        Email = booking.User.Email,
                        Phone = booking.User.PhoneNumber,
                        Address = new Application.Common.Models.PaymentModels.Address
                        {
                            Line1 = booking.User.Address.Street,
                            City = booking.User.Address.City,
                            State = booking.User.Address.State,
                            Country = booking.User.Address.Country,
                            PostalCode = booking.User.Address.PostalCode
                        }
                    };
                }

                invoice.PdfData = GenerateInvoicePdf(invoice);

                _logger.LogInformation("Invoice generated: {InvoiceNumber} for booking: {BookingId}",
                    invoice.InvoiceNumber, bookingId);

                return Result<InvoiceDocument>.Success(invoice);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating invoice for booking: {BookingId}", bookingId);
                return Result<InvoiceDocument>.Failure("An error occurred while generating the invoice");
            }
        }

        public async Task<Result<InvoiceDocument>> GenerateReceiptAsync(Guid paymentId)
        {
            try
            {
                var paymentSpec = new PaymentWithDetailsSpecification(paymentId);
                var payment = await _paymentRepository.GetBySpecAsync(paymentSpec);

                if (payment == null)
                {
                    return Result<InvoiceDocument>.Failure("Payment not found");
                }

                var receipt = new InvoiceDocument
                {
                    Id = Guid.NewGuid(),
                    InvoiceNumber = GenerateReceiptNumber(),
                    BookingId = payment.BookingId,
                    PaymentId = payment.Id,
                    CustomerName = payment.Booking.User.FullName,
                    CustomerEmail = payment.Booking.User.Email,
                    InvoiceDate = payment.CompletedAt ?? DateTime.UtcNow,
                    DueDate = payment.CompletedAt ?? DateTime.UtcNow,
                    Status = InvoiceStatus.Paid,
                    Currency = payment.Amount.Currency,
                    CreatedAt = DateTime.UtcNow,
                    PaidAt = payment.CompletedAt
                };

                // Generate line items from booking tickets
                var lineItems = new List<InvoiceLineItem>();
                foreach (var ticket in payment.Booking.Tickets)
                {
                    lineItems.Add(new InvoiceLineItem
                    {
                        Description = $"{ticket.TicketType.Name} - {payment.Booking.Event.Title}",
                        Quantity = 1,
                        UnitPrice = ticket.Price,
                        TotalPrice = ticket.Price
                    });
                }

                receipt.LineItems = lineItems;
                receipt.Total = payment.Amount;
                receipt.Subtotal = payment.Amount; // Assuming payment amount includes tax

                // Generate PDF receipt
                receipt.PdfData = GenerateReceiptPdf(receipt);

                _logger.LogInformation("Receipt generated: {ReceiptNumber} for payment: {PaymentId}",
                    receipt.InvoiceNumber, paymentId);

                return Result<InvoiceDocument>.Success(receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating receipt for payment: {PaymentId}", paymentId);
                return Result<InvoiceDocument>.Failure("An error occurred while generating the receipt");
            }
        }

        public async Task<Result<InvoiceDocument>> SendInvoiceEmailAsync(Guid bookingId, string email)
        {
            try
            {
                var invoiceResult = await GenerateInvoiceAsync(bookingId);
                if (!invoiceResult.IsSuccess)
                {
                    return Result<InvoiceDocument>.Failure(invoiceResult.Error);
                }

                var invoice = invoiceResult.Value;

                var subject = $"Invoice {invoice!.InvoiceNumber} - Event Registration";
                var body = GenerateInvoiceEmailBody(invoice);

                var emailResult = await _emailService.SendEmailAsync(email, subject, body);

                if (emailResult.IsSuccess)
                {
                    _logger.LogInformation("Invoice email sent: {InvoiceNumber} to: {Email}",
                        invoice.InvoiceNumber, email);
                    return Result<InvoiceDocument>.Success(invoice); // ✅ return full success result
                }
                else
                {
                    return Result<InvoiceDocument>.Failure(emailResult.Error); // ✅ wrap error in correct generic type
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending invoice email for booking: {BookingId}", bookingId);
                return Result<InvoiceDocument>.Failure("An error occurred while sending the invoice email");
            }
        }


        public async Task<Result<byte[]>> GetInvoicePdfAsync(Guid invoiceId)
        {
            try
            {
                // Simplify For Now
                throw new NotImplementedException("PDF retrieval will be implemented with invoice storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving invoice PDF: {InvoiceId}", invoiceId);
                return Result<byte[]>.Failure("An error occurred while retrieving the invoice PDF");
            }
        }

        public async Task<Result<InvoiceDocument>> UpdateInvoiceStatusAsync(Guid invoiceId, InvoiceStatus status)
        {
            try
            {
                // Simplify For Now
                throw new NotImplementedException("Invoice status updates will be implemented with invoice storage");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating invoice status: {InvoiceId}", invoiceId);
                return Result<InvoiceDocument>.Failure("An error occurred while updating the invoice status");
            }
        }

        private string GenerateInvoiceNumber()
        {
            return $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

        private string GenerateReceiptNumber()
        {
            return $"RCP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
        }

        private byte[] GenerateInvoicePdf(InvoiceDocument invoice)
        {
            try
            {
                var document = QuestPDF.Fluent.Document.Create(container =>
                {
                    container.Page(page =>
                    {
                        page.Size(PageSizes.A4);
                        page.Margin(50);

                        page.Header().Row(row =>
                        {
                            row.RelativeItem().Column(column =>
                            {
                                column.Item().Text("INVOICE").FontSize(28).Bold();
                                column.Item().Text($"Invoice #{invoice.InvoiceNumber}").FontSize(16);
                                column.Item().Text($"Date: {invoice.InvoiceDate:yyyy-MM-dd}").FontSize(12);
                                column.Item().Text($"Due Date: {invoice.DueDate:yyyy-MM-dd}").FontSize(12);
                            });

                            row.ConstantItem(100).Height(50).Placeholder();
                        });

                        page.Content().Column(column =>
                        {
                            column.Spacing(20);

                            // Bill To section
                            column.Item().Row(row =>
                            {
                                row.RelativeItem().Column(billTo =>
                                {
                                    billTo.Item().Text("Bill To:").FontSize(14).Bold();
                                    billTo.Item().Text(invoice.CustomerName).FontSize(12);
                                    billTo.Item().Text(invoice.CustomerEmail).FontSize(12);

                                    if (invoice.BillingAddress?.Address != null)
                                    {
                                        var addr = invoice.BillingAddress.Address;
                                        if (!string.IsNullOrEmpty(addr.Line1))
                                            billTo.Item().Text(addr.Line1).FontSize(12);
                                        if (!string.IsNullOrEmpty(addr.City))
                                            billTo.Item().Text($"{addr.City}, {addr.State} {addr.PostalCode}").FontSize(12);
                                        if (!string.IsNullOrEmpty(addr.Country))
                                            billTo.Item().Text(addr.Country).FontSize(12);
                                    }
                                });
                            });

                            // Line items table
                            column.Item().Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(3);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                    columns.RelativeColumn(1);
                                });

                                table.Header(header =>
                                {
                                    header.Cell().Text("Description").Bold();
                                    header.Cell().Text("Qty").Bold();
                                    header.Cell().Text("Unit Price").Bold();
                                    header.Cell().Text("Total").Bold();
                                });

                                foreach (var item in invoice.LineItems)
                                {
                                    table.Cell().Text(item.Description);
                                    table.Cell().Text(item.Quantity.ToString());
                                    table.Cell().Text($"{item.UnitPrice.Amount:C}");
                                    table.Cell().Text($"{item.TotalPrice.Amount:C}");
                                }
                            });

                            // Totals
                            column.Item().AlignRight().Column(totals =>
                            {
                                totals.Item().Row(row =>
                                {
                                    row.ConstantItem(100).Text("Subtotal:");
                                    row.ConstantItem(100).Text($"{invoice.Subtotal.Amount:C}").AlignRight();
                                });

                                if (invoice.TaxAmount != null)
                                {
                                    totals.Item().Row(row =>
                                    {
                                        row.ConstantItem(100).Text("Tax:");
                                        row.ConstantItem(100).Text($"{invoice.TaxAmount.Amount:C}").AlignRight();
                                    });
                                }

                                totals.Item().Row(row =>
                                {
                                    row.ConstantItem(100).Text("Total:").Bold();
                                    row.ConstantItem(100).Text($"{invoice.Total.Amount:C}").Bold().AlignRight();
                                });
                            });

                            // Notes
                            if (!string.IsNullOrEmpty(invoice.Notes))
                            {
                                column.Item().Text("Notes:").Bold();
                                column.Item().Text(invoice.Notes);
                            }
                        });

                        page.Footer().AlignCenter().Text(x =>
                        {
                            x.CurrentPageNumber();
                            x.Span(" / ");
                            x.TotalPages();
                        });
                    });
                });

                return document.GeneratePdf();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating PDF for invoice: {InvoiceNumber}", invoice.InvoiceNumber);
                return Array.Empty<byte>();
            }
        }

        private byte[] GenerateReceiptPdf(InvoiceDocument receipt)
        {
            return GenerateInvoicePdf(receipt);
        }

        private string GenerateInvoiceEmailBody(InvoiceDocument invoice)
        {
            return $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #007bff;'>Invoice {invoice.InvoiceNumber}</h2>
                    <p>Dear {invoice.CustomerName},</p>
                    <p>Thank you for registering for our event. Please find your invoice attached.</p>
                    
                    <div style='background-color: #f8f9fa; padding: 15px; border-radius: 4px; margin: 20px 0;'>
                        <h3>Invoice Details:</h3>
                        <p><strong>Invoice Number:</strong> {invoice.InvoiceNumber}</p>
                        <p><strong>Invoice Date:</strong> {invoice.InvoiceDate:yyyy-MM-dd}</p>
                        <p><strong>Due Date:</strong> {invoice.DueDate:yyyy-MM-dd}</p>
                        <p><strong>Total Amount:</strong> {invoice.Total.Amount:C} {invoice.Currency}</p>
                    </div>
                    
                    <p>Please make payment by the due date to complete your registration.</p>
                    
                    <p>If you have any questions, please don't hesitate to contact us.</p>
                    
                    <p>Best regards,<br>The Event Management Team</p>
                </div>
            </body>
            </html>";
        }
    }
}
