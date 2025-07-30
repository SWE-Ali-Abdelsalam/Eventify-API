using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;

namespace Eventify.Domain.Specifications.BookingSpecifications
{
    public class PendingApprovalBookingsSpecification : BaseSpecification<Booking>
    {
        public PendingApprovalBookingsSpecification()
            : base(b => b.Status == BookingStatus.Pending && b.RequiresApproval)
        {
            AddInclude(b => b.Event);
            AddInclude(b => b.User);
            ApplyOrderBy(b => b.BookingDate);
        }
    }
}
