using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Entities.Sponsors;
using Eventify.Domain.Entities.Supporting;
using Eventify.Domain.Entities.Tickets;
using Eventify.Domain.Entities.Users;
using Eventify.Domain.ValueObjects;
using Eventify.Shared.Enums;

namespace Eventify.Domain.Entities.Events
{
    public class Event : BaseEntity, IAuditableEntity, ISoftDeletable
    {
        public string Title { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string? ShortDescription { get; private set; }
        public EventStatus Status { get; private set; } = EventStatus.Draft;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateRange EventDates { get; private set; } = null!;
        public DateTime? RegistrationOpenDate { get; private set; }
        public DateTime? RegistrationCloseDate { get; private set; }
        public int MaxCapacity { get; private set; }
        public int CurrentRegistrations { get; private set; }
        public bool IsPublic { get; private set; } = true;
        public bool RequiresApproval { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? WebsiteUrl { get; private set; }
        public string? Tags { get; private set; }
        public string? CustomFields { get; private set; } // JSON string for flexibility
        public bool IsVirtual { get; private set; }
        public string? VirtualMeetingUrl { get; private set; }
        public string? VirtualMeetingPassword { get; private set; }

        // Foreign Keys
        public Guid OrganizerId { get; private set; }
        public Guid CategoryId { get; private set; }
        public Guid VenueId { get; private set; }

        // Audit properties
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Soft delete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Navigation properties
        public User Organizer { get; private set; } = null!;
        public EventCategory Category { get; private set; } = null!;
        public Venue Venue { get; private set; } = null!;
        public ICollection<EventSession> Sessions { get; private set; } = new List<EventSession>();
        public ICollection<TicketType> TicketTypes { get; private set; } = new List<TicketType>();
        public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();
        public ICollection<EventSponsor> EventSponsors { get; private set; } = new List<EventSponsor>();
        public ICollection<EventReview> Reviews { get; private set; } = new List<EventReview>();
        public ICollection<EventDocument> Documents { get; private set; } = new List<EventDocument>();
        public ICollection<EventImage> Images { get; private set; } = new List<EventImage>();

        private Event() { } // EF Core

        public Event(string title, string description, DateTime startDate, DateTime endDate,
                     Guid organizerId, Guid categoryId, Guid venueId, int maxCapacity, bool isVirtual = false)
        {
            Title = title;
            Description = description;
            EventDates = new DateRange(startDate, endDate);
            OrganizerId = organizerId;
            CategoryId = categoryId;
            VenueId = venueId;
            MaxCapacity = maxCapacity;
            IsVirtual = isVirtual;
            CurrentRegistrations = 0;
        }

        public void UpdateDetails(string title, string description, string? shortDescription,
                                 DateTime startDate, DateTime endDate, int maxCapacity, bool isVirtual)
        {
            Title = title;
            Description = description;
            ShortDescription = shortDescription;
            EventDates = new DateRange(startDate, endDate);
            MaxCapacity = maxCapacity;
            IsVirtual = isVirtual;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateVenue(Guid venueId)
        {
            VenueId = venueId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetRegistrationPeriod(DateTime openDate, DateTime closeDate)
        {
            if (openDate >= closeDate)
                throw new ArgumentException("Registration open date must be before close date");

            RegistrationOpenDate = openDate;
            RegistrationCloseDate = closeDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Publish()
        {
            if (Status == EventStatus.Draft)
            {
                Status = EventStatus.Published;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Cancel()
        {
            if (Status == EventStatus.Published)
            {
                Status = EventStatus.Cancelled;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void Complete()
        {
            if (Status == EventStatus.Published && DateTime.UtcNow > EventDates.EndDate)
            {
                Status = EventStatus.Completed;
                UpdatedAt = DateTime.UtcNow;
            }
        }

        public void SetVisibility(bool isPublic)
        {
            IsPublic = isPublic;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetApprovalRequirement(bool requiresApproval)
        {
            RequiresApproval = requiresApproval;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateCapacity(int newCapacity)
        {
            if (newCapacity < CurrentRegistrations)
                throw new InvalidOperationException("Cannot reduce capacity below current registrations");

            MaxCapacity = newCapacity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementRegistrations(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));

            if (CurrentRegistrations + count > MaxCapacity)
                throw new InvalidOperationException("Not enough capacity for requested registrations");

            CurrentRegistrations += count;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DecrementRegistrations(int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (CurrentRegistrations - count < 0)
                throw new InvalidOperationException("Cannot decrement below zero");

            CurrentRegistrations -= count;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool HasAvailableSpots => CurrentRegistrations < MaxCapacity;
        public int AvailableSpots => MaxCapacity - CurrentRegistrations;
        public bool IsRegistrationOpen => RegistrationOpenDate <= DateTime.UtcNow &&
                                         DateTime.UtcNow <= RegistrationCloseDate;
        public bool IsUpcoming => DateTime.UtcNow < EventDates.StartDate;
        public bool IsOngoing => EventDates.Contains(DateTime.UtcNow);
        public bool IsCompleted => DateTime.UtcNow > EventDates.EndDate;
    }
}
