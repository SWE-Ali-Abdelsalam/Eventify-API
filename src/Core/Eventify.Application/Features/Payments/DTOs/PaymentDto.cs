namespace Eventify.Application.Features.Payments.DTOs
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Method { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal? RefundedAmount { get; set; }
        public string? ExternalTransactionId { get; set; }
        public DateTime? ProcessedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? FailureReason { get; set; }
        public BookingSummaryDto Booking { get; set; } = null!;
        public List<PaymentRefundDto> Refunds { get; set; } = new();
    }
}
