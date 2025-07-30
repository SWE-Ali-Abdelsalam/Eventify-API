using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Common.Models.PaymentModels
{
    public class InvoiceResult
    {
        public string InvoiceId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Money Amount { get; set; } = null!;
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime DueDate { get; set; }
        public string? InvoiceUrl { get; set; }
        public string? InvoicePdf { get; set; }
    }
}
