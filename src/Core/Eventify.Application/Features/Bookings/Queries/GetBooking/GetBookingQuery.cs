using Eventify.Application.Common;
using Eventify.Application.Features.Bookings.DTOs;

namespace Eventify.Application.Features.Bookings.Queries.GetBooking
{
    public class GetBookingQuery : BaseQuery<Result<BookingDto>>
    {
        public Guid BookingId { get; set; }
        public string RequestedBy { get; set; } = string.Empty;
    }
}
