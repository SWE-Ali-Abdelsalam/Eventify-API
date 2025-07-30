using Eventify.Application.Common;
using Eventify.Application.Features.Events.DTOs;
using Eventify.Shared.Enums;

namespace Eventify.Application.Features.Events.Queries.GetEvents
{
    public class GetEventsQuery : BaseQuery<Result<PaginatedResult<EventSummaryDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public EventStatus? Status { get; set; }
        public bool PublicOnly { get; set; }
        public string? SortBy { get; set; } = "StartDate";
        public bool SortDescending { get; set; } = false;
    }
}
