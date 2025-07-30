namespace Eventify.Application.Features.Admin.DTOs
{
    public class BookingStatsDto
    {
        public int TotalBookings { get; set; }
        public int ConfirmedBookings { get; set; }
        public int PendingBookings { get; set; }
        public int CancelledBookings { get; set; }
        public int BookingsThisMonth { get; set; }
        public double AverageTicketsPerBooking { get; set; }
        public double BookingConversionRate { get; set; }
    }
}
