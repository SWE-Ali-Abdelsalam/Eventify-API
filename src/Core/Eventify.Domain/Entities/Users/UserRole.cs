using Eventify.Domain.Common;
using Eventify.Shared.Enums;

namespace Eventify.Domain.Entities.Users
{
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; private set; }
        public UserRoleType Role { get; private set; }

        public DateTime AssignedAt { get; private set; }
        public string? AssignedBy { get; private set; }
        public bool IsActive { get; private set; } = true;
        public DateTime? ExpiresAt { get; private set; }
        public string? Notes { get; private set; }

        // Navigation properties
        public User User { get; private set; } = null!;
        private UserRole() { }
        public UserRole(Guid userId, UserRoleType role, string? assignedBy = null, DateTime? expiresAt = null, string? notes = null)
        {
            UserId = userId;
            Role = role;
            AssignedAt = DateTime.UtcNow;
            AssignedBy = assignedBy;
            ExpiresAt = expiresAt;
            Notes = notes;
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

        public void ExtendExpiration(DateTime newExpirationDate)
        {
            if (newExpirationDate <= DateTime.UtcNow)
                throw new ArgumentException("Expiration date must be in the future", nameof(newExpirationDate));

            ExpiresAt = newExpirationDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveExpiration()
        {
            ExpiresAt = null;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes;
            UpdatedAt = DateTime.UtcNow;
        }

        public bool IsValid => IsActive && (!ExpiresAt.HasValue || ExpiresAt > DateTime.UtcNow);

        public bool IsExpired => ExpiresAt.HasValue && ExpiresAt <= DateTime.UtcNow;

        public string RoleName => Role.ToString();
    }
}
