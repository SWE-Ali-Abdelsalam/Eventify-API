namespace Eventify.Application.Features.Events.DTOs
{
    public class EventDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public bool IsVirtual { get; set; }
        public string? VirtualMeetingUrl { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentRegistrations { get; set; }
        public int AvailableSpots { get; set; }
        public bool RequiresApproval { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationCloseDate { get; set; }
        public bool IsRegistrationOpen { get; set; }
        public string? ImageUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Tags { get; set; }
        public DateTime CreatedAt { get; set; }
        public EventCategoryDto Category { get; set; } = null!;
        public EventOrganizerDto Organizer { get; set; } = null!;
        public VenueDetailDto? Venue { get; set; }
        public List<EventSessionDto> Sessions { get; set; } = new();
        public List<TicketTypeDetailDto> TicketTypes { get; set; } = new();
        public List<EventImageDto> Images { get; set; } = new();
        public List<EventDocumentDto> Documents { get; set; } = new();
    }
}
