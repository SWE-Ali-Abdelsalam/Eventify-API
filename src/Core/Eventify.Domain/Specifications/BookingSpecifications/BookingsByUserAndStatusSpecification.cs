using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;

namespace Eventify.Domain.Specifications.BookingSpecifications
{
    public class BookingsByUserAndStatusSpecification : BaseSpecification<Booking>
    {
        public BookingsByUserAndStatusSpecification(Guid userId, BookingStatus status)
            : base(b => b.UserId == userId && b.Status == status)
        {
            AddInclude(b => b.Event);
            AddInclude(b => b.User);
            AddInclude(b => b.Tickets);
            AddInclude(b => b.Payments);
            ApplyOrderByDescending(b => b.BookingDate);
        }
    }
}
