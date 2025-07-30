using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Features.Payments.DTOs
{
    public class RefundPaymentResult
    {
        public Guid PaymentId { get; set; }
        public string RefundId { get; set; } = string.Empty;
        public Money RefundAmount { get; set; } = null!;
        public string Status { get; set; } = string.Empty;
        public DateTime ProcessedAt { get; set; }
    }
}
