using Eventify.Domain.Common;
using Eventify.Domain.Entities.Users;
using Eventify.Shared.Enums;

namespace Eventify.Domain.Specifications.UserSpecifications
{
    public class UsersByRoleSpecification : BaseSpecification<User>
    {
        public UsersByRoleSpecification(UserRoleType role)
            : base(u => u.UserRoles.Any(ur => ur.Role == role))
        {
            AddInclude(u => u.UserRoles);
            ApplyOrderBy(u => u.FirstName);
        }
    }
}
