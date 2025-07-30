namespace Eventify.Application.Common.Models.PaymentModels
{
    public class ProcessPaymentRequest
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public string PaymentMethodId { get; set; } = string.Empty;
        public BillingDetails? BillingDetails { get; set; }
        public string? ReceiptEmail { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
