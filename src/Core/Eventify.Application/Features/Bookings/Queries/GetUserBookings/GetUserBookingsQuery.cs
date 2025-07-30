using Eventify.Application.Common;
using Eventify.Application.Features.Bookings.DTOs;
using Eventify.Domain.Entities.Bookings;

namespace Eventify.Application.Features.Bookings.Queries.GetUserBookings
{
    public class GetUserBookingsQuery : BaseQuery<Result<List<UserBookingDto>>>
    {
        public Guid UserId { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public BookingStatus? Status { get; set; }
    }
}
