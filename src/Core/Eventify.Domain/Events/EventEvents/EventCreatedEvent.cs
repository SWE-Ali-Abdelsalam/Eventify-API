using Eventify.Domain.Common;

namespace Eventify.Domain.Events.EventEvents
{
    public class EventCreatedEvent : DomainEvent
    {
        public Guid EventId { get; }
        public string Title { get; }
        public Guid OrganizerId { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }

        public EventCreatedEvent(Guid eventId, string title, Guid organizerId, DateTime startDate, DateTime endDate)
        {
            EventId = eventId;
            Title = title;
            OrganizerId = organizerId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
