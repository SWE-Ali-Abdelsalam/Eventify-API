using Eventify.Application.Common;

namespace Eventify.Application.Features.Events.Commands.DeleteEvent
{
    public class DeleteEventCommand : BaseCommand<Result>
    {
        public Guid EventId { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
    }
}
