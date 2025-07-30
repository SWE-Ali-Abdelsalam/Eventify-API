using Eventify.Application.Common;

namespace Eventify.Application.Features.Events.Commands.PublishEvent
{
    public class PublishEventCommand : BaseCommand<Result>
    {
        public Guid EventId { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
    }
}
