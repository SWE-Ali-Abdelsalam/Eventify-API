using Microsoft.AspNetCore.Identity;

namespace Eventify.Domain.Entities.Identity
{
    public class ApplicationRoleClaim : IdentityRoleClaim<Guid>
    {
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string? CreatedBy { get; set; }

        // Navigation properties
        public ApplicationRole Role { get; set; } = null!;
    }
}
