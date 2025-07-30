using Eventify.Domain.Common;

namespace Eventify.Domain.Events.EventEvents
{
    public class EventCancelledEvent : DomainEvent
    {
        public Guid EventId { get; }
        public string Title { get; }
        public Guid OrganizerId { get; }
        public string? CancellationReason { get; }

        public EventCancelledEvent(Guid eventId, string title, Guid organizerId, string? cancellationReason = null)
        {
            EventId = eventId;
            Title = title;
            OrganizerId = organizerId;
            CancellationReason = cancellationReason;
        }
    }
}
