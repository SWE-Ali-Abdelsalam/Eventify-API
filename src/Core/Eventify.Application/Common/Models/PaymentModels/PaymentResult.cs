using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Common.Models.PaymentModels
{
    public class PaymentResult
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Money Amount { get; set; } = null!;
        public string Currency { get; set; } = string.Empty;
        public string? ReceiptUrl { get; set; }
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
        public string? FailureMessage { get; set; }
        public BillingDetails? BillingDetails { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
