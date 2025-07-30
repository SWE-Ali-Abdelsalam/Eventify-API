namespace Eventify.Application.Features.Payments.DTOs
{
    public class BookingSummaryDto
    {
        public Guid Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int TotalTickets { get; set; }
    }
}
