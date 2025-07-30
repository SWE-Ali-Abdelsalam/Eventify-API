using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Entities.Users;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Entities.Bookings
{
    public enum BookingStatus
    {
        Pending = 1,
        Confirmed = 2,
        Cancelled = 3,
        Completed = 4,
        Refunded = 5,
        WaitingList = 6
    }

    public class Booking : BaseEntity, IAuditableEntity
    {
        public Guid EventId { get; private set; }
        public Guid UserId { get; private set; }
        public string BookingNumber { get; private set; } = string.Empty;
        public BookingStatus Status { get; private set; } = BookingStatus.Pending;
        public DateTime BookingDate { get; private set; } = DateTime.UtcNow;
        public DateTime? ConfirmationDate { get; private set; }
        public DateTime? CancellationDate { get; private set; }
        public string? CancellationReason { get; private set; }
        public int TotalTickets { get; private set; }
        public Money TotalAmount { get; private set; } = null!;
        public Money? DiscountAmount { get; private set; }
        public string? PromoCode { get; private set; }
        public string? SpecialRequests { get; private set; }
        public string? AttendeeInformation { get; private set; } // JSON string
        public bool RequiresApproval { get; private set; }
        public DateTime? ApprovalDate { get; private set; }
        public string? ApprovedBy { get; private set; }
        public string? RejectionReason { get; private set; }
        public string? CheckInCode { get; private set; }
        public DateTime? CheckInTime { get; private set; }
        public bool IsGroupBooking { get; private set; }
        public int GroupSize { get; private set; }

        // Audit properties
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Navigation properties
        public Event Event { get; private set; } = null!;
        public User User { get; private set; } = null!;
        public ICollection<BookingTicket> Tickets { get; private set; } = new List<BookingTicket>();
        public ICollection<Payment> Payments { get; private set; } = new List<Payment>();
        public ICollection<BookingModification> Modifications { get; private set; } = new List<BookingModification>();

        private Booking() { } // EF Core

        public Booking(Guid eventId, Guid userId, Money totalAmount, int totalTickets,
                       bool isGroupBooking = false, string? promoCode = null)
        {
            EventId = eventId;
            UserId = userId;
            TotalAmount = totalAmount;
            TotalTickets = totalTickets;
            IsGroupBooking = isGroupBooking;
            GroupSize = isGroupBooking ? totalTickets : 1;
            PromoCode = promoCode;
            BookingNumber = GenerateBookingNumber();
            CheckInCode = GenerateCheckInCode();
        }

        private string GenerateBookingNumber()
        {
            return $"BK{DateTime.UtcNow:yyyyMMdd}{Id.ToString("N")[..8].ToUpper()}";
        }

        private string GenerateCheckInCode()
        {
            return Guid.NewGuid().ToString("N")[..8].ToUpper();
        }

        public void Confirm()
        {
            if (Status == BookingStatus.Pending || Status == BookingStatus.WaitingList)
            {
                Status = BookingStatus.Confirmed;
                ConfirmationDate = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Cancel(string reason)
        {
            if (Status == BookingStatus.Confirmed || Status == BookingStatus.Pending)
            {
                Status = BookingStatus.Cancelled;
                CancellationDate = DateTime.UtcNow;
                CancellationReason = reason;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Complete()
        {
            if (Status == BookingStatus.Confirmed)
            {
                Status = BookingStatus.Completed;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Refund()
        {
            if (Status == BookingStatus.Cancelled || Status == BookingStatus.Confirmed)
            {
                Status = BookingStatus.Refunded;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void MoveToWaitingList()
        {
            if (Status == BookingStatus.Pending)
            {
                Status = BookingStatus.WaitingList;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void ApplyDiscount(Money discountAmount)
        {
            DiscountAmount = discountAmount;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Approve(string approvedBy)
        {
            if (RequiresApproval && Status == BookingStatus.Pending)
            {
                Status = BookingStatus.Confirmed;
                ApprovalDate = DateTime.UtcNow;
                ApprovedBy = approvedBy;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Reject(string rejectionReason)
        {
            if (RequiresApproval && Status == BookingStatus.Pending)
            {
                Status = BookingStatus.Cancelled;
                RejectionReason = rejectionReason;
                CancellationDate = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void CheckIn()
        {
            if (Status == BookingStatus.Confirmed && CheckInTime == null)
            {
                CheckInTime = DateTime.UtcNow;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void UpdateSpecialRequests(string? specialRequests)
        {
            SpecialRequests = specialRequests;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAttendeeInformation(string attendeeInformationJson)
        {
            AttendeeInformation = attendeeInformationJson;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetRequiresApproval()
        {
            RequiresApproval = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public Money FinalAmount => TotalAmount.Subtract(DiscountAmount ?? new Money(0, TotalAmount.Currency));
        public bool IsCheckedIn => CheckInTime.HasValue;
        public bool CanBeCancelled => Status == BookingStatus.Confirmed || Status == BookingStatus.Pending;
        public bool CanBeRefunded => Status == BookingStatus.Cancelled && Payments.Any(p => p.Status == PaymentStatus.Completed);
    }
}
