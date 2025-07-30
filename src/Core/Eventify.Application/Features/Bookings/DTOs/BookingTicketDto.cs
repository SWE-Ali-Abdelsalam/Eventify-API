namespace Eventify.Application.Features.Bookings.DTOs
{
    public class BookingTicketDto
    {
        public Guid Id { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string AttendeeFullName { get; set; } = string.Empty;
        public string AttendeeEmail { get; set; } = string.Empty;
        public string? AttendeePhone { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? QRCode { get; set; }
        public DateTime? CheckInTime { get; set; }
        public string TicketTypeName { get; set; } = string.Empty;
    }
}
