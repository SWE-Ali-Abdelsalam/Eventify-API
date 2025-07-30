using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;

namespace Eventify.Domain.Specifications.BookingSpecifications
{
    public class BookingsByStatusSpecification : BaseSpecification<Booking>
    {
        public BookingsByStatusSpecification(BookingStatus status)
            : base(b => b.Status == status)
        {
            AddInclude(b => b.Event);
            AddInclude(b => b.User);
            ApplyOrderByDescending(b => b.BookingDate);
        }
    }
}
