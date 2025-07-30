using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;

namespace Eventify.Domain.Specifications.EventSpecifications
{
    public class EventWithDetailsSpecification : BaseSpecification<Event>
    {
        public EventWithDetailsSpecification(Guid eventId)
            : base(e => e.Id == eventId)
        {
            AddInclude(e => e.Category);
            AddInclude(e => e.Organizer);
            AddInclude(e => e.Venue);
            AddInclude(e => e.Sessions);
            AddInclude(e => e.TicketTypes);
            AddInclude(e => e.Images);
            AddInclude(e => e.Documents);
        }
    }
}
