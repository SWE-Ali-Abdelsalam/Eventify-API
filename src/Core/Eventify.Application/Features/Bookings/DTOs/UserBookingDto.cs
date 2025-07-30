namespace Eventify.Application.Features.Bookings.DTOs
{
    public class UserBookingDto
    {
        public Guid Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public int TotalTickets { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string EventTitle { get; set; } = string.Empty;
        public DateTime EventStartDate { get; set; }
        public string? EventImageUrl { get; set; }
        public bool RequiresApproval { get; set; }
        public bool CanBeCancelled { get; set; }
        public bool IsCheckedIn { get; set; }
    }
}
