using Eventify.Domain.Common;

namespace Eventify.Domain.Events.EventEvents
{
    public class EventPublishedEvent : DomainEvent
    {
        public Guid EventId { get; }
        public string Title { get; }
        public Guid OrganizerId { get; }

        public EventPublishedEvent(Guid eventId, string title, Guid organizerId)
        {
            EventId = eventId;
            Title = title;
            OrganizerId = organizerId;
        }
    }
}
