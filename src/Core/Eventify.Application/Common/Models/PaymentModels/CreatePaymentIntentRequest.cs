using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Common.Models.PaymentModels
{
    public class CreatePaymentIntentRequest
    {
        public Money Amount { get; set; } = null!;
        public string Currency { get; set; } = "USD";
        public string? CustomerId { get; set; }
        public string? PaymentMethodId { get; set; }
        public bool CaptureMethod { get; set; } = true; // true = automatic, false = manual
        public string? Description { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
        public string? ReceiptEmail { get; set; }
        public BillingDetails? BillingDetails { get; set; }
    }
}
