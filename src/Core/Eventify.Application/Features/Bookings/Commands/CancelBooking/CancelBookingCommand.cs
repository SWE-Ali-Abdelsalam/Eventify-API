using Eventify.Application.Common;

namespace Eventify.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommand : BaseCommand<Result>
    {
        public Guid BookingId { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
        public string? CancellationReason { get; set; }
    }
}
