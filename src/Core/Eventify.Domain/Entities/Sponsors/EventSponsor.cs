using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Entities.Sponsors
{
    public class EventSponsor : BaseEntity, IAuditableEntity
    {
        public Guid EventId { get; private set; }
        public Guid SponsorProfileId { get; private set; }
        public SponsorshipLevel Level { get; private set; }
        public Money SponsorshipAmount { get; private set; } = null!;
        public string? CustomPackageDetails { get; private set; } // JSON string
        public string? Benefits { get; private set; } // JSON string
        public string? Deliverables { get; private set; } // JSON string
        public DateTime SponsorshipStartDate { get; private set; }
        public DateTime SponsorshipEndDate { get; private set; }
        public bool IsApproved { get; private set; }
        public DateTime? ApprovalDate { get; private set; }
        public string? ApprovedBy { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string? Notes { get; private set; }

        // Audit properties
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Navigation properties
        public Event Event { get; private set; } = null!;
        public SponsorProfile SponsorProfile { get; private set; } = null!;

        private EventSponsor() { } // EF Core

        public EventSponsor(Guid eventId, Guid sponsorProfileId, SponsorshipLevel level,
                           Money sponsorshipAmount, DateTime startDate, DateTime endDate)
        {
            EventId = eventId;
            SponsorProfileId = sponsorProfileId;
            Level = level;
            SponsorshipAmount = sponsorshipAmount;
            SponsorshipStartDate = startDate;
            SponsorshipEndDate = endDate;
        }

        public void UpdateSponsorshipDetails(SponsorshipLevel level, Money sponsorshipAmount,
                                            string? customPackageDetails, string? benefits,
                                            string? deliverables)
        {
            Level = level;
            SponsorshipAmount = sponsorshipAmount;
            CustomPackageDetails = customPackageDetails;
            Benefits = benefits;
            Deliverables = deliverables;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Approve(string approvedBy)
        {
            IsApproved = true;
            ApprovalDate = DateTime.UtcNow;
            ApprovedBy = approvedBy;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateDates(DateTime startDate, DateTime endDate)
        {
            SponsorshipStartDate = startDate;
            SponsorshipEndDate = endDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsCurrentlyActive => IsActive && IsApproved &&
                                       DateTime.UtcNow >= SponsorshipStartDate &&
                                       DateTime.UtcNow <= SponsorshipEndDate;
    }
}
