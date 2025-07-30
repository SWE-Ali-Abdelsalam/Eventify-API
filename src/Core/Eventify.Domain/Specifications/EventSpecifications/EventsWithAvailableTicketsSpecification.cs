using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Shared.Enums;

namespace Eventify.Domain.Specifications.EventSpecifications
{
    public class EventsWithAvailableTicketsSpecification : BaseSpecification<Event>
    {
        public EventsWithAvailableTicketsSpecification()
            : base(e => e.Status == EventStatus.Published &&
                       e.IsPublic &&
                       e.CurrentRegistrations < e.MaxCapacity &&
                       e.RegistrationOpenDate <= DateTime.UtcNow &&
                       e.RegistrationCloseDate >= DateTime.UtcNow)
        {
            AddInclude(e => e.Organizer);
            AddInclude(e => e.Category);
            AddInclude(e => e.TicketTypes);
            ApplyOrderBy(e => e.EventDates.StartDate);
        }
    }
}
