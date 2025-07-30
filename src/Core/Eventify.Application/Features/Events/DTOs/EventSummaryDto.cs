namespace Eventify.Application.Features.Events.DTOs
{
    public class EventSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public bool IsVirtual { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentRegistrations { get; set; }
        public int AvailableSpots { get; set; }
        public string? ImageUrl { get; set; }
        public EventCategoryDto Category { get; set; } = null!;
        public EventOrganizerDto Organizer { get; set; } = null!;
        public VenueSummaryDto? Venue { get; set; }
        public List<TicketTypeSummaryDto> TicketTypes { get; set; } = new();
    }
}
