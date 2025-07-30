using Eventify.Domain.Common;
using Eventify.Domain.ValueObjects;
using Eventify.Domain.Entities.Users;

namespace Eventify.Domain.Entities.Sponsors
{
    public enum SponsorshipLevel
    {
        Bronze = 1,
        Silver = 2,
        Gold = 3,
        Platinum = 4,
        Diamond = 5,
        Custom = 6
    }

    public class SponsorProfile : BaseEntity, IAuditableEntity
    {
        public Guid UserId { get; private set; }
        public string CompanyName { get; private set; } = string.Empty;
        public string Industry { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string? Website { get; private set; }
        public string? LogoUrl { get; private set; }
        public Address? Address { get; private set; }
        public string? ContactPersonName { get; private set; }
        public string? ContactPersonEmail { get; private set; }
        public string? ContactPersonPhone { get; private set; }
        public string? SocialMediaLinks { get; private set; } // JSON string
        public string? MarketingMaterials { get; private set; } // JSON string
        public bool IsVerified { get; private set; }
        public DateTime? VerificationDate { get; private set; }
        public bool IsActive { get; private set; } = true;

        // Audit properties
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Navigation properties
        public User User { get; private set; } = null!;
        public ICollection<EventSponsor> EventSponsors { get; private set; } = new List<EventSponsor>();

        private SponsorProfile() { } // EF Core

        public SponsorProfile(Guid userId, string companyName, string industry, string description)
        {
            UserId = userId;
            CompanyName = companyName;
            Industry = industry;
            Description = description;
        }

        public void UpdateDetails(string companyName, string industry, string description,
                                 string? website, string? logoUrl)
        {
            CompanyName = companyName;
            Industry = industry;
            Description = description;
            Website = website;
            LogoUrl = logoUrl;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateContactInfo(string? contactPersonName, string? contactPersonEmail,
                                     string? contactPersonPhone)
        {
            ContactPersonName = contactPersonName;
            ContactPersonEmail = contactPersonEmail;
            ContactPersonPhone = contactPersonPhone;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAddress(Address address)
        {
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Verify()
        {
            IsVerified = true;
            VerificationDate = DateTime.UtcNow;
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

        public void UpdateSocialMediaLinks(string socialMediaLinks)
        {
            SocialMediaLinks = socialMediaLinks;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateMarketingMaterials(string marketingMaterials)
        {
            MarketingMaterials = marketingMaterials;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
