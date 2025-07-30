using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Shared.Enums;

namespace Eventify.Domain.Specifications.EventSpecifications
{
    public class UpcomingEventsSpecification : BaseSpecification<Event>
    {
        public UpcomingEventsSpecification(DateTime fromDate)
            : base(e => e.EventDates.StartDate >= fromDate && e.Status == EventStatus.Published && e.IsPublic)
        {
            AddInclude(e => e.Organizer);
            AddInclude(e => e.Category);
            AddInclude(e => e.Venue);
            ApplyOrderBy(e => e.EventDates.StartDate);
        }
    }
}
