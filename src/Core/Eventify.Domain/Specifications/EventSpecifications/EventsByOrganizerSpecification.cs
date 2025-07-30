using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;

namespace Eventify.Domain.Specifications.EventSpecifications
{
    public class EventsByOrganizerSpecification : BaseSpecification<Event>
    {
        public EventsByOrganizerSpecification(Guid organizerId)
            : base(e => e.OrganizerId == organizerId)
        {
            AddInclude(e => e.Category);
            AddInclude(e => e.Venue);
            AddInclude(e => e.TicketTypes);
            ApplyOrderByDescending(e => e.CreatedAt);
        }
    }
}
