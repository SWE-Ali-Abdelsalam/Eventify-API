using Eventify.Domain.Common;
using Eventify.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;

namespace Eventify.Domain.Entities.Identity
{
    public class ApplicationUser : IdentityUser<Guid>, IAuditableEntity, ISoftDeletable
    {
        public Guid? EventManagementUserId { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? LastLoginAt { get; set; }
        public string? LastLoginIp { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEndDate { get; set; }
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? EmailVerificationTokenExpiry { get; set; }

        // Audit properties
        public string? CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedAt { get; set; }

        // Soft delete properties
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }
        public string? DeletedBy { get; set; }

        // Navigation properties
        public User? EventManagementUser { get; set; }
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
        public ICollection<UserLoginHistory> LoginHistory { get; set; } = new List<UserLoginHistory>();

        public void UpdateRefreshToken(string refreshToken, DateTime expiryTime)
        {
            RefreshToken = refreshToken;
            RefreshTokenExpiryTime = expiryTime;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void ClearRefreshToken()
        {
            RefreshToken = null;
            RefreshTokenExpiryTime = null;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void RecordLogin(string ipAddress)
        {
            LastLoginAt = DateTime.UtcNow;
            LastLoginIp = ipAddress;
            FailedLoginAttempts = 0;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void RecordFailedLogin()
        {
            FailedLoginAttempts++;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetEmailVerificationToken(string token, DateTime expiry)
        {
            EmailVerificationToken = token;
            EmailVerificationTokenExpiry = expiry;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void VerifyEmail()
        {
            EmailConfirmed = true;
            EmailVerificationToken = null;
            EmailVerificationTokenExpiry = null;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void SetPasswordResetToken(string token, DateTime expiry)
        {
            PasswordResetToken = token;
            PasswordResetTokenExpiry = expiry;
            LastModifiedAt = DateTime.UtcNow;
        }

        public void ClearPasswordResetToken()
        {
            PasswordResetToken = null;
            PasswordResetTokenExpiry = null;
            LastModifiedAt = DateTime.UtcNow;
        }

        public bool IsRefreshTokenValid => RefreshToken != null &&
                                          RefreshTokenExpiryTime.HasValue &&
                                          RefreshTokenExpiryTime > DateTime.UtcNow;

        public bool IsEmailVerificationTokenValid => EmailVerificationToken != null &&
                                                   EmailVerificationTokenExpiry.HasValue &&
                                                   EmailVerificationTokenExpiry > DateTime.UtcNow;

        public bool IsPasswordResetTokenValid => PasswordResetToken != null &&
                                               PasswordResetTokenExpiry.HasValue &&
                                               PasswordResetTokenExpiry > DateTime.UtcNow;
    }
}
