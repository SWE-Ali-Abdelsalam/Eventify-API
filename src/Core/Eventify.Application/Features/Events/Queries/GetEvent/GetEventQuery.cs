using Eventify.Application.Common;
using Eventify.Application.Features.Events.DTOs;

namespace Eventify.Application.Features.Events.Queries.GetEvent
{
    public class GetEventQuery : BaseQuery<Result<EventDetailDto>>
    {
        public Guid EventId { get; set; }
    }
}
