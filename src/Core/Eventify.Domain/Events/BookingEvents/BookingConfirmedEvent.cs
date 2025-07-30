using Eventify.Domain.Common;

namespace Eventify.Domain.Events.BookingEvents
{
    public class BookingConfirmedEvent : DomainEvent
    {
        public Guid BookingId { get; }
        public Guid EventId { get; }
        public Guid UserId { get; }
        public string BookingNumber { get; }

        public BookingConfirmedEvent(Guid bookingId, Guid eventId, Guid userId, string bookingNumber)
        {
            BookingId = bookingId;
            EventId = eventId;
            UserId = userId;
            BookingNumber = bookingNumber;
        }
    }
}
