using Eventify.Domain.Common;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Events.PaymentEvents
{
    public class PaymentProcessedEvent : DomainEvent
    {
        public Guid PaymentId { get; }
        public Guid BookingId { get; }
        public string PaymentNumber { get; }
        public Money Amount { get; }
        public string ExternalTransactionId { get; }

        public PaymentProcessedEvent(Guid paymentId, Guid bookingId, string paymentNumber, Money amount, string externalTransactionId)
        {
            PaymentId = paymentId;
            BookingId = bookingId;
            PaymentNumber = paymentNumber;
            Amount = amount;
            ExternalTransactionId = externalTransactionId;
        }
    }
}
