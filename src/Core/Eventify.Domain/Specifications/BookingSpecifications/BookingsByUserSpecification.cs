using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;

namespace Eventify.Domain.Specifications.BookingSpecifications
{
    public class BookingsByUserSpecification : BaseSpecification<Booking>
    {
        public BookingsByUserSpecification(Guid userId)
            : base(b => b.UserId == userId)
        {
            AddInclude(b => b.Event);
            AddInclude(b => b.Event.Organizer);
            AddInclude(b => b.Event.Category);
            AddInclude(b => b.Tickets);
            AddInclude(b => b.Payments);
            ApplyOrderByDescending(b => b.BookingDate);
        }
    }
}
