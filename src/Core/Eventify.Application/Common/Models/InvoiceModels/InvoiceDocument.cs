using Eventify.Application.Common.Models.PaymentModels;
using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Common.Models.InvoiceModels
{
    public class InvoiceDocument
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid BookingId { get; set; }
        public Guid? PaymentId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerEmail { get; set; } = string.Empty;
        public BillingDetails? BillingAddress { get; set; }
        public DateTime InvoiceDate { get; set; }
        public DateTime DueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public Money Subtotal { get; set; } = null!;
        public Money? TaxAmount { get; set; }
        public Money Total { get; set; } = null!;
        public string Currency { get; set; } = string.Empty;
        public List<InvoiceLineItem> LineItems { get; set; } = new();
        public string? Notes { get; set; }
        public string? Terms { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaidAt { get; set; }
        public string? ExternalInvoiceId { get; set; } // Stripe invoice ID
        public string? PaymentUrl { get; set; }
        public byte[]? PdfData { get; set; }
    }
}
