using Eventify.Domain.Common;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Events.BookingEvents
{
    public class BookingCreatedEvent : DomainEvent
    {
        public Guid BookingId { get; }
        public Guid EventId { get; }
        public Guid UserId { get; }
        public string BookingNumber { get; }
        public Money TotalAmount { get; }
        public int TotalTickets { get; }

        public BookingCreatedEvent(Guid bookingId, Guid eventId, Guid userId, string bookingNumber, Money totalAmount, int totalTickets)
        {
            BookingId = bookingId;
            EventId = eventId;
            UserId = userId;
            BookingNumber = bookingNumber;
            TotalAmount = totalAmount;
            TotalTickets = totalTickets;
        }
    }
}
