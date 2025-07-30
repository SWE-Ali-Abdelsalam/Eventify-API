using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Common.Models.PaymentModels
{
    public class PaymentIntentResult
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Money Amount { get; set; } = null!;
        public string Currency { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
