using Eventify.Domain.Common;

namespace Eventify.Domain.Events.BookingEvents
{
    public class BookingCancelledEvent : DomainEvent
    {
        public Guid BookingId { get; }
        public Guid EventId { get; }
        public Guid UserId { get; }
        public string BookingNumber { get; }
        public string? CancellationReason { get; }

        public BookingCancelledEvent(Guid bookingId, Guid eventId, Guid userId, string bookingNumber, string? cancellationReason = null)
        {
            BookingId = bookingId;
            EventId = eventId;
            UserId = userId;
            BookingNumber = bookingNumber;
            CancellationReason = cancellationReason;
        }
    }
}
