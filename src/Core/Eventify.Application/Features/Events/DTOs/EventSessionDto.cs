namespace Eventify.Application.Features.Events.DTOs
{
    public class EventSessionDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string? Location { get; set; }
        public string? SpeakerName { get; set; }
        public string? SpeakerBio { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentRegistrations { get; set; }
        public bool IsRequired { get; set; }
    }
}
