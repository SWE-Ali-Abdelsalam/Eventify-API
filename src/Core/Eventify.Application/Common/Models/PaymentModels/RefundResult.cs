using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Common.Models.PaymentModels
{
    public class RefundResult
    {
        public string RefundId { get; set; } = string.Empty;
        public string PaymentIntentId { get; set; } = string.Empty;
        public Money Amount { get; set; } = null!;
        public string Status { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
