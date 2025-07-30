using Eventify.Application.Common;
using Eventify.Application.Features.Bookings.DTOs;

namespace Eventify.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommand : BaseCommand<Result<CreateBookingResult>>
    {
        public Guid EventId { get; set; }
        public Guid UserId { get; set; } // Set by controller from current user
        public List<TicketSelection> TicketSelections { get; set; } = new();
        public string? PromoCode { get; set; }
        public string? SpecialRequests { get; set; }
        public List<AttendeeInfo> AttendeeInformation { get; set; } = new();
        public bool IsGroupBooking { get; set; }
    }
}
