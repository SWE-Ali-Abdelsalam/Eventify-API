namespace Eventify.Application.Features.Bookings.DTOs
{
    public class PaymentSummaryDto
    {
        public Guid Id { get; set; }
        public string PaymentNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime? CompletedAt { get; set; }
    }
}
