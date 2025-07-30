using Eventify.Application.Common;

namespace Eventify.Application.Features.Bookings.Commands.ConfirmBooking
{
    public class ConfirmBookingCommand : BaseCommand<Result>
    {
        public Guid BookingId { get; set; }
        public string ApprovedBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
