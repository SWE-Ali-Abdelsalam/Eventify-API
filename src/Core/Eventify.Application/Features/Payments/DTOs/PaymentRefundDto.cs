namespace Eventify.Application.Features.Payments.DTOs
{
    public class PaymentRefundDto
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime RefundedAt { get; set; }
    }
}
