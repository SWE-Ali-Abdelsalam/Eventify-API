using Eventify.Application.Common;
using Eventify.Application.Features.Events.DTOs;

namespace Eventify.Application.Features.Events.Queries.GetEventsByOrganizer
{
    public class GetEventsByOrganizerQuery : BaseQuery<Result<List<EventSummaryDto>>>
    {
        public Guid OrganizerId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
