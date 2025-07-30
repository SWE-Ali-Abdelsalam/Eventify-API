using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Shared.Enums;

namespace Eventify.Domain.Specifications.EventSpecifications
{
    public class EventsByCategorySpecification : BaseSpecification<Event>
    {
        public EventsByCategorySpecification(Guid categoryId)
            : base(e => e.CategoryId == categoryId && e.Status == EventStatus.Published && e.IsPublic)
        {
            AddInclude(e => e.Organizer);
            AddInclude(e => e.Venue);
            AddInclude(e => e.TicketTypes);
            ApplyOrderBy(e => e.EventDates.StartDate);
        }
    }
}
