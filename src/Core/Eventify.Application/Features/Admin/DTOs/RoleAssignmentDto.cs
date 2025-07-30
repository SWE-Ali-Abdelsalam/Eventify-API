using Eventify.Domain.Entities.Users;
using Eventify.Shared.Enums;

namespace Eventify.Application.Features.Admin.DTOs
{
    public class RoleAssignmentDto
    {
        public UserRoleType Role { get; set; }
        public bool Assign { get; set; } // true to assign, false to remove
        public DateTime? ExpiresAt { get; set; }
        public string? Notes { get; set; }
    }
}
