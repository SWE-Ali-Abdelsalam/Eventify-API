using Eventify.Domain.Common;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Events.PaymentEvents
{
    public class PaymentCompletedEvent : DomainEvent
    {
        public Guid PaymentId { get; }
        public Guid BookingId { get; }
        public string PaymentNumber { get; }
        public Money Amount { get; }

        public PaymentCompletedEvent(Guid paymentId, Guid bookingId, string paymentNumber, Money amount)
        {
            PaymentId = paymentId;
            BookingId = bookingId;
            PaymentNumber = paymentNumber;
            Amount = amount;
        }
    }
}
