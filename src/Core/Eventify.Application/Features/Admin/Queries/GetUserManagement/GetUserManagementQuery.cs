using Eventify.Application.Common;
using Eventify.Application.Features.Admin.DTOs;
using Eventify.Domain.Entities.Users;
using Eventify.Shared.Enums;

namespace Eventify.Application.Features.Admin.Queries.GetUserManagement
{
    public class GetUserManagementQuery : BaseQuery<Result<PaginatedResult<UserManagementDto>>>
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? SearchTerm { get; set; }
        public UserRoleType? RoleFilter { get; set; }
        public bool? IsActiveFilter { get; set; }
        public bool? IsVerifiedFilter { get; set; }
        public string? SortBy { get; set; } = "CreatedAt";
        public bool SortDescending { get; set; } = true;
    }
}
