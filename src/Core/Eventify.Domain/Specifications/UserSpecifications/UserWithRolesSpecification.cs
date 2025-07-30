using Eventify.Domain.Common;
using Eventify.Domain.Entities.Users;

namespace Eventify.Domain.Specifications.UserSpecifications
{
    public class UserWithRolesSpecification : BaseSpecification<User>
    {
        public UserWithRolesSpecification(Guid userId)
            : base(u => u.Id == userId)
        {
            AddInclude(u => u.UserRoles);
        }
    }
}
