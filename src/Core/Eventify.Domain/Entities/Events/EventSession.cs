using Eventify.Domain.Common;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Entities.Events
{
    public class EventSession : BaseEntity
    {
        public Guid EventId { get; private set; }
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public DateRange SessionDates { get; private set; } = null!;
        public string? Location { get; private set; }
        public string? SpeakerName { get; private set; }
        public string? SpeakerBio { get; private set; }
        public string? SpeakerImageUrl { get; private set; }
        public int MaxCapacity { get; private set; }
        public int CurrentRegistrations { get; private set; }
        public bool IsRequired { get; private set; }
        public int SortOrder { get; private set; }

        // Navigation properties
        public Event Event { get; private set; } = null!;

        private EventSession() { } // EF Core

        public EventSession(Guid eventId, string title, string description,
                            DateTime startTime, DateTime endTime, int maxCapacity, bool isRequired = false)
        {
            EventId = eventId;
            Title = title;
            Description = description;
            SessionDates = new DateRange(startTime, endTime);
            MaxCapacity = maxCapacity;
            IsRequired = isRequired;
            CurrentRegistrations = 0;
        }

        public void UpdateDetails(string title, string description, DateTime startTime, DateTime endTime,
                                 string? location, int maxCapacity)
        {
            Title = title;
            Description = description;
            SessionDates = new DateRange(startTime, endTime);
            Location = location;
            MaxCapacity = maxCapacity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateSpeaker(string? speakerName, string? speakerBio, string? speakerImageUrl)
        {
            SpeakerName = speakerName;
            SpeakerBio = speakerBio;
            SpeakerImageUrl = speakerImageUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetRequired(bool isRequired)
        {
            IsRequired = isRequired;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateSortOrder(int sortOrder)
        {
            SortOrder = sortOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasAvailableSpots => CurrentRegistrations < MaxCapacity;
        public int AvailableSpots => MaxCapacity - CurrentRegistrations;
    }
}
