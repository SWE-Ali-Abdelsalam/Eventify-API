using Eventify.Application.Features.Payments.DTOs;

namespace Eventify.Application.Features.Bookings.DTOs
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public string BookingNumber { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public DateTime? ConfirmationDate { get; set; }
        public int TotalTickets { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public decimal? DiscountAmount { get; set; }
        public string? PromoCode { get; set; }
        public string? SpecialRequests { get; set; }
        public bool RequiresApproval { get; set; }
        public string? CheckInCode { get; set; }
        public DateTime? CheckInTime { get; set; }
        public bool IsGroupBooking { get; set; }
        public EventSummaryDto Event { get; set; } = null!;
        public UserSummaryDto User { get; set; } = null!;
        public List<BookingTicketDto> Tickets { get; set; } = new();
        public List<PaymentSummaryDto> Payments { get; set; } = new();
    }
}
