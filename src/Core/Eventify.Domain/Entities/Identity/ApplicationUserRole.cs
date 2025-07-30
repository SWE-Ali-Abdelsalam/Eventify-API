using Microsoft.AspNetCore.Identity;

namespace Eventify.Domain.Entities.Identity
{
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;
        public string? AssignedBy { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;
        public ApplicationRole Role { get; set; } = null!;
    }
}
