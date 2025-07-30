using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Shared.Enums;

namespace Eventify.Domain.Specifications.EventSpecifications
{
    public class EventsByStatusSpecification : BaseSpecification<Event>
    {
        public EventsByStatusSpecification(EventStatus status)
            : base(e => e.Status == status)
        {
            AddInclude(e => e.Organizer);
            AddInclude(e => e.Category);
            AddInclude(e => e.Venue);
            ApplyOrderBy(e => e.EventDates.StartDate);
        }
    }
}
