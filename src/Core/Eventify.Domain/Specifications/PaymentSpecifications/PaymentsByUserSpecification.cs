using Eventify.Domain.Common;
using Eventify.Domain.Entities.Payments;

namespace Eventify.Domain.Specifications.PaymentSpecifications
{
    public class PaymentsByUserSpecification : BaseSpecification<Payment>
    {
        public PaymentsByUserSpecification(Guid userId)
            : base(p => p.Booking.UserId == userId)
        {
            AddInclude(p => p.Booking);
            AddInclude(p => p.Booking.Event);
            AddInclude(p => p.Refunds);
            ApplyOrderByDescending(p => p.CreatedAt);
        }
    }
}
