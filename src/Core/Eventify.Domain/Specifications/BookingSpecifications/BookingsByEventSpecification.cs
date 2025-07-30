using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;

namespace Eventify.Domain.Specifications.BookingSpecifications
{
    public class BookingsByEventSpecification : BaseSpecification<Booking>
    {
        public BookingsByEventSpecification(Guid eventId)
            : base(b => b.EventId == eventId)
        {
            AddInclude(b => b.User);
            AddInclude(b => b.Tickets);
            AddInclude(b => b.Payments);
            ApplyOrderByDescending(b => b.BookingDate);
        }
    }
}
