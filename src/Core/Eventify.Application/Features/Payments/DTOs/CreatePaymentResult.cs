using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Features.Payments.DTOs
{
    public class CreatePaymentResult
    {
        public Guid PaymentId { get; set; }
        public string PaymentIntentId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public Money Amount { get; set; } = null!;
        public bool RequiresAction { get; set; }
    }
}
