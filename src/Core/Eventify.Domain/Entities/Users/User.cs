using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.Entities.Sponsors;
using Eventify.Domain.Entities.Supporting;
using Eventify.Domain.ValueObjects;
using Eventify.Shared.Enums;

namespace Eventify.Domain.Entities.Users
{
    public class User : BaseEntity, IAuditableEntity, ISoftDeletable
    {
        public string FirstName { get; private set; } = string.Empty;
        public string LastName { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string? PhoneNumber { get; private set; }
        public string? ProfileImageUrl { get; private set; }
        public DateTime? DateOfBirth { get; private set; }
        public Address? Address { get; private set; }
        public string? Bio { get; private set; }
        public string? Website { get; private set; }
        public string? Company { get; private set; }
        public string? JobTitle { get; private set; }
        public bool IsEmailVerified { get; private set; }
        public bool IsPhoneVerified { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public bool TwoFactorEnabled { get; private set; }
        public string? TimeZone { get; private set; }
        public string? Language { get; private set; }
        public bool IsActive { get; private set; } = true;

        // Audit properties
        public string? CreatedBy { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Soft delete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Navigation properties
        public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();
        public ICollection<Event> OrganizedEvents { get; private set; } = new List<Event>();
        public ICollection<Booking> Bookings { get; private set; } = new List<Booking>();
        public ICollection<SponsorProfile> SponsorProfiles { get; private set; } = new List<SponsorProfile>();
        public ICollection<EventReview> EventReviews { get; private set; } = new List<EventReview>();
        public ICollection<UserPreference> Preferences { get; private set; } = new List<UserPreference>();

        private User() { } // EF Core

        public User(string firstName, string lastName, string email)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email.ToLowerInvariant();
            Language = "en";
            TimeZone = "UTC";
        }

        public string FullName => $"{FirstName} {LastName}";

        public void UpdateProfile(string firstName, string lastName, string? phoneNumber, DateTime? dateOfBirth, string? bio, string? website, string? company, string? jobTitle)
        {
            FirstName = firstName;
            LastName = lastName;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
            Bio = bio;
            Website = website;
            Company = company;
            JobTitle = jobTitle;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAddress(Address address)
        {
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }

        public void VerifyEmail()
        {
            IsEmailVerified = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void VerifyPhone()
        {
            IsPhoneVerified = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void EnableTwoFactor()
        {
            TwoFactorEnabled = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DisableTwoFactor()
        {
            TwoFactorEnabled = false;
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

        public bool HasRole(UserRoleType role)
        {
            return UserRoles.Any(ur => ur.Role == role && ur.IsActive);
        }

        public void AddRole(UserRoleType role)
        {
            if (!HasRole(role))
            {
                UserRoles.Add(new UserRole(Id, role));
            }
        }

        public void RemoveRole(UserRoleType role)
        {
            var userRole = UserRoles.FirstOrDefault(ur => ur.Role == role);
            if (userRole != null)
            {
                UserRoles.Remove(userRole);
            }
        }

        public void UpdateTimeZone(string timeZone)
        {
            TimeZone = timeZone;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateLanguage(string language)
        {
            Language = language;
            UpdatedAt = DateTime.UtcNow;
        }

        public IEnumerable<UserRoleType> GetActiveRoles()
        {
            return UserRoles.Where(ur => ur.IsActive).Select(ur => ur.Role);
        }

        public IEnumerable<string> GetActiveRoleNames()
        {
            return UserRoles
                .Where(ur => ur.IsActive)
                .Select(ur => ur.Role.ToString())
                .Distinct();
        }

        public bool IsInAnyRole(params UserRoleType[] roles)
        {
            return roles.Any(role => HasRole(role));
        }

        public bool HasAllRoles(params UserRoleType[] roles)
        {
            return roles.All(role => HasRole(role));
        }
    }
}
