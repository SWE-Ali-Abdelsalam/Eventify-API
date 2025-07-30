using Eventify.Application.Common;

namespace Eventify.Application.Features.Events.Commands.UpdateEvent
{
    public class UpdateEventCommand : BaseCommand<Result>
    {
        public Guid EventId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? ShortDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Guid VenueId { get; set; }
        public int MaxCapacity { get; set; }
        public bool IsVirtual { get; set; }
        public string? VirtualMeetingUrl { get; set; }
        public bool IsPublic { get; set; } = true;
        public bool RequiresApproval { get; set; }
        public DateTime? RegistrationOpenDate { get; set; }
        public DateTime? RegistrationCloseDate { get; set; }
        public string? ImageUrl { get; set; }
        public string? WebsiteUrl { get; set; }
        public string? Tags { get; set; }

        // Set by controller
        public string RequestedBy { get; set; } = string.Empty;
    }
}
