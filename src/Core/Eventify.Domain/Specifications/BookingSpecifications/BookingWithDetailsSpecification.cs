using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;

namespace Eventify.Domain.Specifications.BookingSpecifications
{
    public class BookingWithDetailsSpecification : BaseSpecification<Booking>
    {
        public BookingWithDetailsSpecification(Guid bookingId)
            : base(b => b.Id == bookingId)
        {
            AddInclude(b => b.User);
            AddInclude(b => b.Event);
            AddInclude(b => b.Tickets);
            AddInclude(b => b.Tickets.Select(t => t.TicketType));
            AddInclude(b => b.Payments);
        }
    }
}
