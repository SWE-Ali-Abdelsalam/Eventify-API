using Eventify.Domain.Common;
using Eventify.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eventify.Domain.Entities.Bookings
{
    public enum ModificationType
    {
        TicketQuantityChange = 1,
        TicketTypeChange = 2,
        AttendeeChange = 3,
        Cancellation = 4,
        Refund = 5
    }

    public class BookingModification : BaseEntity
    {
        public Guid BookingId { get; private set; }
        public ModificationType Type { get; private set; }
        public string Description { get; private set; } = string.Empty;
        public string? OldValue { get; private set; }
        public string? NewValue { get; private set; }
        public Money? AmountChange { get; private set; }
        public string RequestedBy { get; private set; } = string.Empty;
        public DateTime RequestedAt { get; private set; } = DateTime.UtcNow;
        public string? ProcessedBy { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        // Navigation properties
        public Booking Booking { get; private set; } = null!;

        private BookingModification() { } // EF Core

        public BookingModification(Guid bookingId, ModificationType type, string description,
                                  string requestedBy, string? oldValue = null, string? newValue = null,
                                  Money? amountChange = null)
        {
            BookingId = bookingId;
            Type = type;
            Description = description;
            RequestedBy = requestedBy;
            OldValue = oldValue;
            NewValue = newValue;
            AmountChange = amountChange;
        }

        public void Process(string processedBy)
        {
            ProcessedBy = processedBy;
            ProcessedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsProcessed => ProcessedAt.HasValue;
    }
}
