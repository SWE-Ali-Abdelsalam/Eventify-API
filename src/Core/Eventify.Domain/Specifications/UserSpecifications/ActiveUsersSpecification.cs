using Eventify.Domain.Common;
using Eventify.Domain.Entities.Users;

namespace Eventify.Domain.Specifications.UserSpecifications
{
    public class ActiveUsersSpecification : BaseSpecification<User>
    {
        public ActiveUsersSpecification()
            : base(u => u.IsActive && u.IsEmailVerified)
        {
            ApplyOrderBy(u => u.FirstName);
        }
    }
}
