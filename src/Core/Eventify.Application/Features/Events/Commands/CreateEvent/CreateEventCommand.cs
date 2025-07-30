using Eventify.Application.Common;

namespace Eventify.Application.Features.Events.Commands.CreateEvent
{
    public class CreateEventCommand : BaseCommand<Result<Guid>>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid CategoryId { get; set; }
        public Guid VenueId { get; set; }
        public int MaxCapacity { get; set; }
        public bool IsVirtual { get; set; }
        public bool IsPublic { get; set; } = true;
        public bool RequiresApproval { get; set; }

        // This will be set by the controller from the authenticated user
        public Guid OrganizerId { get; set; }
    }
}
