using Eventify.Application.Common;
using Eventify.Application.Features.Admin.DTOs;

namespace Eventify.Application.Features.Admin.Commands.ManageUserRoles
{
    public class ManageUserRolesCommand : BaseCommand<Result>
    {
        public Guid UserId { get; set; }
        public List<RoleAssignmentDto> RoleAssignments { get; set; } = new();
        public string? RequestedBy { get; set; }
    }
}
