using Eventify.Domain.Common;
using Eventify.Domain.Entities.Tickets;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Entities.Bookings
{
    public class BookingTicket : BaseEntity
    {
        public Guid BookingId { get; private set; }
        public Guid TicketTypeId { get; private set; }
        public string TicketNumber { get; private set; } = string.Empty;
        public string AttendeeFirstName { get; private set; } = string.Empty;
        public string AttendeeLastName { get; private set; } = string.Empty;
        public string AttendeeEmail { get; private set; } = string.Empty;
        public string? AttendeePhone { get; private set; }
        public Money Price { get; private set; } = null!;
        public string? QRCode { get; private set; }
        public DateTime? CheckInTime { get; private set; }
        public string? CheckInLocation { get; private set; }
        public string? SpecialRequests { get; private set; }
        public bool IsTransferred { get; private set; }
        public string? TransferredTo { get; private set; }
        public DateTime? TransferDate { get; private set; }

        // Navigation properties
        public Booking Booking { get; private set; } = null!;
        public TicketType TicketType { get; private set; } = null!;

        private BookingTicket() { } // EF Core

        public BookingTicket(Guid bookingId, Guid ticketTypeId, string attendeeFirstName,
                            string attendeeLastName, string attendeeEmail, Money price)
        {
            BookingId = bookingId;
            TicketTypeId = ticketTypeId;
            AttendeeFirstName = attendeeFirstName;
            AttendeeLastName = attendeeLastName;
            AttendeeEmail = attendeeEmail;
            Price = price;
            TicketNumber = GenerateTicketNumber();
            QRCode = GenerateQRCode();
        }

        private string GenerateTicketNumber()
        {
            return $"TK{DateTime.UtcNow:yyyyMMdd}{Id.ToString("N")[..8].ToUpper()}";
        }

        private string GenerateQRCode()
        {
            return $"{TicketNumber}_{Id}_{BookingId}";
        }

        public void CheckIn(string location)
        {
            if (CheckInTime == null)
            {
                CheckInTime = DateTime.UtcNow;
                CheckInLocation = location;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Transfer(string transferredTo)
        {
            if (!IsTransferred)
            {
                IsTransferred = true;
                TransferredTo = transferredTo;
                TransferDate = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UpdateAttendeeInfo(string firstName, string lastName, string email, string? phone)
        {
            AttendeeFirstName = firstName;
            AttendeeLastName = lastName;
            AttendeeEmail = email;
            AttendeePhone = phone;
            UpdatedAt = DateTime.UtcNow;
        }

        public string AttendeeFullName => $"{AttendeeFirstName} {AttendeeLastName}";
        public bool IsCheckedIn => CheckInTime.HasValue;
    }
}
