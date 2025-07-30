using Eventify.Domain.Common;
using Eventify.Domain.Entities.Payments;

namespace Eventify.Domain.Specifications.PaymentSpecifications
{
    public class PaymentsByBookingSpecification : BaseSpecification<Payment>
    {
        public PaymentsByBookingSpecification(Guid bookingId)
            : base(p => p.BookingId == bookingId)
        {
            AddInclude(p => p.Refunds);
            ApplyOrderByDescending(p => p.CreatedAt);
        }
    }
}
