namespace Eventify.Application.Features.Admin.DTOs
{
    public class EventStatsDto
    {
        public int TotalEvents { get; set; }
        public int PublishedEvents { get; set; }
        public int DraftEvents { get; set; }
        public int CompletedEvents { get; set; }
        public int CancelledEvents { get; set; }
        public int EventsThisMonth { get; set; }
        public double AverageCapacity { get; set; }
        public double AverageRegistrationRate { get; set; }
    }
}
