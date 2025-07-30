using Microsoft.AspNetCore.Identity;

namespace Eventify.Domain.Entities.Identity
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        public bool IsSystemRole { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();
        public ICollection<ApplicationRoleClaim> RoleClaims { get; set; } = new List<ApplicationRoleClaim>();

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName, string? description = null, bool isSystemRole = false) : base(roleName)
        {
            Description = description;
            IsSystemRole = isSystemRole;
        }
    }
}
