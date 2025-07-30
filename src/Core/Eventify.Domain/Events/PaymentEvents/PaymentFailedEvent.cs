using Eventify.Domain.Common;

namespace Eventify.Domain.Events.PaymentEvents
{
    public class PaymentFailedEvent : DomainEvent
    {
        public Guid PaymentId { get; }
        public Guid BookingId { get; }
        public string PaymentNumber { get; }
        public string FailureReason { get; }

        public PaymentFailedEvent(Guid paymentId, Guid bookingId, string paymentNumber, string failureReason)
        {
            PaymentId = paymentId;
            BookingId = bookingId;
            PaymentNumber = paymentNumber;
            FailureReason = failureReason;
        }
    }
}
