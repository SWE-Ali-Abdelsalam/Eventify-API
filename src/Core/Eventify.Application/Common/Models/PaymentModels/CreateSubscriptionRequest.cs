namespace Eventify.Application.Common.Models.PaymentModels
{
    public class CreateSubscriptionRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        public string PriceId { get; set; } = string.Empty;
        public int Quantity { get; set; } = 1;
        public DateTime? TrialEnd { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
