using Eventify.Domain.Common;
using Eventify.Domain.Entities.Payments;

namespace Eventify.Domain.Specifications.PaymentSpecifications
{
    public class PaymentByExternalIdSpecification : BaseSpecification<Payment>
    {
        public PaymentByExternalIdSpecification(string externalTransactionId)
            : base(p => p.ExternalTransactionId == externalTransactionId)
        {
            AddInclude(p => p.Booking);
            AddInclude(p => p.Booking.User);
            AddInclude(p => p.Booking.Event);
        }
    }
}
