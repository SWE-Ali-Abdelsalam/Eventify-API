using Eventify.Domain.Entities.Bookings;

namespace Eventify.Application.Features.Bookings.DTOs
{
    public class CreateBookingResult
    {
        public Guid Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public BookingStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int TotalTickets { get; set; }
        public bool RequiresApproval { get; set; }
        public DateTime BookingDate { get; set; }
        public string CheckInCode { get; set; } = string.Empty;
    }
}
