using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Common.Models.PaymentModels;
using Eventify.Domain.ValueObjects;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using Address = Eventify.Application.Common.Models.PaymentModels.Address;

namespace Eventify.Infrastructure.Services;

public class StripePaymentService : IPaymentService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<StripePaymentService> _logger;
    private readonly string _secretKey;
    private readonly string _webhookSecret;

    public StripePaymentService(IConfiguration configuration, ILogger<StripePaymentService> logger)
    {
        _configuration = configuration;
        _logger = logger;
        _secretKey = _configuration["Stripe:SecretKey"] ?? throw new InvalidOperationException("Stripe secret key is required");
        _webhookSecret = _configuration["Stripe:WebhookSecret"] ?? throw new InvalidOperationException("Stripe webhook secret is required");

        StripeConfiguration.ApiKey = _secretKey;
    }

    public async Task<Result<PaymentIntentResult>> CreatePaymentIntentAsync(CreatePaymentIntentRequest request)
    {
        try
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount.Amount * 100), // Convert to cents
                Currency = request.Currency.ToLower(),
                Customer = request.CustomerId,
                PaymentMethod = request.PaymentMethodId,
                CaptureMethod = request.CaptureMethod ? "automatic" : "manual",
                Description = request.Description,
                ReceiptEmail = request.ReceiptEmail,
                Metadata = request.Metadata
            };

            if (request.BillingDetails != null)
            {
                options.Shipping = new ChargeShippingOptions
                {
                    Name = request.BillingDetails.Name,
                    Phone = request.BillingDetails.Phone,
                    Address = MapAddress(request.BillingDetails.Address)
                };
            }

            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);

            var result = new PaymentIntentResult
            {
                PaymentIntentId = paymentIntent.Id,
                ClientSecret = paymentIntent.ClientSecret,
                Status = paymentIntent.Status,
                Amount = new Money((decimal)paymentIntent.Amount / 100, request.Currency),
                Currency = paymentIntent.Currency.ToUpper(),
                CreatedAt = paymentIntent.Created
            };

            _logger.LogInformation("Payment intent created: {PaymentIntentId} for amount: {Amount} {Currency}",
                paymentIntent.Id, result.Amount.Amount, result.Currency);

            return Result<PaymentIntentResult>.Success(result);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error creating payment intent: {Error}", ex.Message);
            return Result<PaymentIntentResult>.Failure($"Payment processing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error creating payment intent");
            return Result<PaymentIntentResult>.Failure("An unexpected error occurred during payment processing");
        }
    }

    public async Task<Result<PaymentResult>> ProcessPaymentAsync(ProcessPaymentRequest request)
    {
        try
        {
            var service = new PaymentIntentService();

            var confirmOptions = new PaymentIntentConfirmOptions
            {
                PaymentMethod = request.PaymentMethodId,
                ReceiptEmail = request.ReceiptEmail
            };

            if (request.BillingDetails != null)
            {
                confirmOptions.PaymentMethodData = new PaymentIntentPaymentMethodDataOptions
                {
                    BillingDetails = new PaymentIntentPaymentMethodDataBillingDetailsOptions
                    {
                        Name = request.BillingDetails.Name,
                        Email = request.BillingDetails.Email,
                        Phone = request.BillingDetails.Phone,
                        Address = new AddressOptions
                        {
                            Line1 = request.BillingDetails.Address?.Line1,
                            Line2 = request.BillingDetails.Address?.Line2,
                            City = request.BillingDetails.Address?.City,
                            State = request.BillingDetails.Address?.State,
                            PostalCode = request.BillingDetails.Address?.PostalCode,
                            Country = request.BillingDetails.Address?.Country
                        }

                    }
                };
            }

            var paymentIntent = await service.ConfirmAsync(request.PaymentIntentId, confirmOptions);

            var result = new PaymentResult
            {
                PaymentIntentId = paymentIntent.Id,
                Status = paymentIntent.Status,
                Amount = new Money((decimal)paymentIntent.Amount / 100, paymentIntent.Currency.ToUpper()),
                Currency = paymentIntent.Currency.ToUpper(),
                ProcessedAt = DateTime.UtcNow,
                Metadata = paymentIntent.Metadata.ToDictionary(x => x.Key, x => x.Value)
            };

            var chargeId = paymentIntent.LatestChargeId;
            if (!string.IsNullOrEmpty(chargeId))
            {
                var chargeService = new ChargeService();
                var fetchedCharge = await chargeService.GetAsync(chargeId);
                result.ReceiptUrl = fetchedCharge.ReceiptUrl;
            }

            if (paymentIntent.LastPaymentError != null)
            {
                result.FailureMessage = paymentIntent.LastPaymentError.Message;
            }

            _logger.LogInformation("Payment processed: {PaymentIntentId} with status: {Status}",
                paymentIntent.Id, paymentIntent.Status);

            return Result<PaymentResult>.Success(result);

        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error processing payment: {Error}", ex.Message);
            return Result<PaymentResult>.Failure($"Payment processing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing payment");
            return Result<PaymentResult>.Failure("An unexpected error occurred during payment processing");
        }
    }

    public async Task<Result<RefundResult>> ProcessRefundAsync(ProcessRefundRequest request)
    {
        try
        {
            var options = new RefundCreateOptions
            {
                PaymentIntent = request.PaymentIntentId,
                Reason = request.Reason switch
                {
                    "duplicate" => "duplicate",
                    "fraudulent" => "fraudulent",
                    "requested_by_customer" => "requested_by_customer",
                    _ => "requested_by_customer"
                },
                Metadata = request.Metadata
            };

            if (request.Amount != null)
            {
                options.Amount = (long)(request.Amount.Amount * 100); // Convert to cents
            }

            var service = new RefundService();
            var refund = await service.CreateAsync(options);

            var result = new RefundResult
            {
                RefundId = refund.Id,
                PaymentIntentId = refund.PaymentIntentId ?? request.PaymentIntentId,
                Amount = new Money((decimal)refund.Amount / 100, refund.Currency.ToUpper()),
                Status = refund.Status,
                Reason = refund.Reason,
                ProcessedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Refund processed: {RefundId} for payment: {PaymentIntentId} amount: {Amount}",
                refund.Id, request.PaymentIntentId, result.Amount.Amount);

            return Result<RefundResult>.Success(result);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error processing refund: {Error}", ex.Message);
            return Result<RefundResult>.Failure($"Refund processing error: {ex.Message}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error processing refund");
            return Result<RefundResult>.Failure("An unexpected error occurred during refund processing");
        }
    }

    public async Task<Result<PaymentResult>> CapturePaymentAsync(string paymentIntentId)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.CaptureAsync(paymentIntentId);

            var result = new PaymentResult
            {
                PaymentIntentId = paymentIntent.Id,
                Status = paymentIntent.Status,
                Amount = new Money((decimal)paymentIntent.Amount / 100, paymentIntent.Currency.ToUpper()),
                Currency = paymentIntent.Currency.ToUpper(),
                ProcessedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Payment captured: {PaymentIntentId}", paymentIntentId);

            return Result<PaymentResult>.Success(result);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error capturing payment: {Error}", ex.Message);
            return Result<PaymentResult>.Failure($"Payment capture error: {ex.Message}");
        }
    }

    public async Task<Result<PaymentResult>> CancelPaymentAsync(string paymentIntentId)
    {
        try
        {
            var service = new PaymentIntentService();
            var paymentIntent = await service.CancelAsync(paymentIntentId);

            var result = new PaymentResult
            {
                PaymentIntentId = paymentIntent.Id,
                Status = paymentIntent.Status,
                Amount = new Money((decimal)paymentIntent.Amount / 100, paymentIntent.Currency.ToUpper()),
                Currency = paymentIntent.Currency.ToUpper(),
                ProcessedAt = DateTime.UtcNow
            };

            _logger.LogInformation("Payment cancelled: {PaymentIntentId}", paymentIntentId);

            return Result<PaymentResult>.Success(result);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error cancelling payment: {Error}", ex.Message);
            return Result<PaymentResult>.Failure($"Payment cancellation error: {ex.Message}");
        }
    }

    public Task<Result<PaymentResult>> ValidateWebhookSignatureAsync(string payload, string signature)
    {
        try
        {
            var stripeEvent = EventUtility.ConstructEvent(payload, signature, _webhookSecret);
            _logger.LogInformation("Webhook signature validated for event type: {EventType}", stripeEvent.Type);
            return Task.FromResult(Result<PaymentResult>.Success(null));
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Webhook signature validation failed: {Error}", ex.Message);
            return Task.FromResult(Result<PaymentResult>.Failure($"Webhook signature validation failed: {ex.Message}"));
        }
    }



    public Task<Result<PaymentResult>> HandleWebhookEventAsync(string eventType, string eventData)
    {
        try
        {
            _logger.LogInformation("Processing webhook event: {EventType}", eventType);

            switch (eventType)
            {
                case "payment_intent.succeeded":
                case "payment_intent.payment_failed":
                case "payment_intent.canceled":
                    _logger.LogInformation("Handled webhook event: {EventType}", eventType);
                    break;
                default:
                    _logger.LogWarning("Unhandled webhook event type: {EventType}", eventType);
                    break;
            }

            return Task.FromResult(Result<PaymentResult>.Success(new PaymentResult()));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling webhook event: {EventType}", eventType);
            return Task.FromResult(Result<PaymentResult>.Failure($"Error handling webhook event: {ex.Message}"));
        }
    }


    public async Task<Result<CustomerResult>> CreateCustomerAsync(CreateCustomerRequest request)
    {
        try
        {
            var options = new CustomerCreateOptions
            {
                Email = request.Email,
                Name = request.Name,
                Phone = request.Phone,
                Metadata = request.Metadata
            };

            if (request.BillingDetails?.Address != null)
            {
                options.Address = MapAddress(request.BillingDetails.Address);
            }

            var service = new CustomerService();
            var customer = await service.CreateAsync(options);

            var result = new CustomerResult
            {
                CustomerId = customer.Id,
                Email = customer.Email,
                Name = customer.Name,
                Phone = customer.Phone,
                CreatedAt = customer.Created
            };

            _logger.LogInformation("Customer created: {CustomerId} for email: {Email}", customer.Id, customer.Email);

            return Result<CustomerResult>.Success(result);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error creating customer: {Error}", ex.Message);
            return Result<CustomerResult>.Failure($"Customer creation error: {ex.Message}");
        }
    }

    public async Task<Result<CustomerResult>> UpdateCustomerAsync(string customerId, UpdateCustomerRequest request)
    {
        try
        {
            var options = new CustomerUpdateOptions
            {
                Email = request.Email,
                Name = request.Name,
                Phone = request.Phone,
                Metadata = request.Metadata
            };

            if (request.BillingDetails?.Address != null)
            {
                options.Address = MapAddress(request.BillingDetails.Address);
            }

            var service = new CustomerService();
            var customer = await service.UpdateAsync(customerId, options);

            var result = new CustomerResult
            {
                CustomerId = customer.Id,
                Email = customer.Email,
                Name = customer.Name,
                Phone = customer.Phone,
                CreatedAt = customer.Created
            };

            _logger.LogInformation("Customer updated: {CustomerId}", customerId);

            return Result<CustomerResult>.Success(result);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error updating customer: {Error}", ex.Message);
            return Result<CustomerResult>.Failure($"Customer update error: {ex.Message}");
        }
    }

    public async Task<Result<InvoiceResult>> CreateInvoiceAsync(CreateInvoiceRequest request)
    {
        try
        {
            // First create invoice items
            var invoiceItemService = new InvoiceItemService();
            foreach (var lineItem in request.LineItems)
            {
                await invoiceItemService.CreateAsync(new InvoiceItemCreateOptions
                {
                    Customer = request.CustomerId,
                    Amount = (long)(lineItem.TotalPrice.Amount * 100),
                    Currency = request.Currency.ToLower(),
                    Description = lineItem.Description
                });
            }

            // Then create the invoice
            var options = new InvoiceCreateOptions
            {
                Customer = request.CustomerId,
                Description = request.Description,
                DueDate = request.DueDate,
                Metadata = request.Metadata,
                AutoAdvance = true
            };

            var service = new Stripe.InvoiceService();
            var invoice = await service.CreateAsync(options);

            // Finalize the invoice to make it payable
            await service.FinalizeInvoiceAsync(invoice.Id);

            var result = new InvoiceResult
            {
                InvoiceId = invoice.Id,
                Status = invoice.Status,
                Amount = new Money((decimal)invoice.Total / 100, invoice.Currency.ToUpper()),
                Currency = invoice.Currency.ToUpper(),
                CreatedAt = invoice.Created,
                DueDate = invoice.DueDate ?? request.DueDate,
                InvoiceUrl = invoice.HostedInvoiceUrl,
                InvoicePdf = invoice.InvoicePdf
            };

            _logger.LogInformation("Invoice created: {InvoiceId} for customer: {CustomerId}", invoice.Id, request.CustomerId);

            return Result<InvoiceResult>.Success(result);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error creating invoice: {Error}", ex.Message);
            return Result<InvoiceResult>.Failure($"Invoice creation error: {ex.Message}");
        }
    }

    public async Task<Result<SubscriptionResult>> CreateSubscriptionAsync(CreateSubscriptionRequest request)
    {
        try
        {
            var options = new SubscriptionCreateOptions
            {
                Customer = request.CustomerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = request.PriceId,
                        Quantity = request.Quantity
                    }
                },
                TrialEnd = request.TrialEnd,
                Metadata = request.Metadata
            };

            var service = new SubscriptionService();
            var subscription = await service.CreateAsync(options);

            var result = new SubscriptionResult
            {
                SubscriptionId = subscription.Id,
                Status = subscription.Status,
                CreatedAt = subscription.Created,
                CurrentPeriodStart = subscription.StartDate,
                CurrentPeriodEnd = subscription.CancelAt ?? subscription.EndedAt
            };



            if (subscription.Items?.Data?.FirstOrDefault()?.Price != null)
            {
                var price = subscription.Items.Data.First().Price;
                result.Amount = new Money((decimal)price.UnitAmount!.Value / 100, price.Currency.ToUpper());
            }

            _logger.LogInformation("Subscription created: {SubscriptionId} for customer: {CustomerId}", subscription.Id, request.CustomerId);

            return Result<SubscriptionResult>.Success(result);
        }
        catch (StripeException ex)
        {
            _logger.LogError(ex, "Stripe error creating subscription: {Error}", ex.Message);
            return Result<SubscriptionResult>.Failure($"Subscription creation error: {ex.Message}");
        }
    }

    private static AddressOptions? MapAddress(Address? address)
{
    if (address == null) return null;

    return new AddressOptions
    {
        Line1 = address.Line1,
        Line2 = address.Line2,
        City = address.City,
        State = address.State,
        PostalCode = address.PostalCode,
        Country = address.Country
    };
}

}