using Eventify.Domain.Common;
using Eventify.Domain.Entities.Payments;

namespace Eventify.Domain.Specifications.PaymentSpecifications
{
    public class PaymentWithDetailsSpecification : BaseSpecification<Payment>
    {
        public PaymentWithDetailsSpecification(Guid paymentId)
            : base(p => p.Id == paymentId)
        {
            AddInclude(p => p.Booking);
            AddInclude(p => p.Booking.User);
            AddInclude(p => p.Booking.Event);
            AddInclude(p => p.Refunds);
        }
    }
}
