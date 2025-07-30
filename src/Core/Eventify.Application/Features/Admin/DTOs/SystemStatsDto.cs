namespace Eventify.Application.Features.Admin.DTOs
{
    public class SystemStatsDto
    {
        public UserStatsDto Users { get; set; } = new();
        public EventStatsDto Events { get; set; } = new();
        public BookingStatsDto Bookings { get; set; } = new();
        public RevenueStatsDto Revenue { get; set; } = new();
        public SystemHealthDto SystemHealth { get; set; } = new();
    }
}
