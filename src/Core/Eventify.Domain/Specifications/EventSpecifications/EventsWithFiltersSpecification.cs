using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Shared.Enums;

namespace Eventify.Domain.Specifications.EventSpecifications
{
    public class EventsWithFiltersSpecification : BaseSpecification<Event>
    {
        public EventsWithFiltersSpecification(
            string? searchTerm = null,
            Guid? categoryId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            EventStatus? status = null,
            bool publicOnly = false)
            : base(BuildCriteriaExpression(searchTerm, categoryId, startDate, endDate, status, publicOnly))
        {
            AddInclude(e => e.Category);
            AddInclude(e => e.Organizer);
            AddInclude(e => e.Venue);
            AddInclude(e => e.TicketTypes);
            ApplyOrderBy(e => e.EventDates.StartDate);
        }

        private static System.Linq.Expressions.Expression<Func<Event, bool>> BuildCriteriaExpression(
            string? searchTerm,
            Guid? categoryId,
            DateTime? startDate,
            DateTime? endDate,
            EventStatus? status,
            bool publicOnly)
        {
            return e =>
                (string.IsNullOrEmpty(searchTerm) || e.Title.Contains(searchTerm) || e.Description.Contains(searchTerm)) &&
                (!categoryId.HasValue || e.CategoryId == categoryId.Value) &&
                (!startDate.HasValue || e.EventDates.StartDate >= startDate.Value) &&
                (!endDate.HasValue || e.EventDates.EndDate <= endDate.Value) &&
                (!status.HasValue || e.Status == status.Value) &&
                (!publicOnly || e.IsPublic);
        }
    }
}
