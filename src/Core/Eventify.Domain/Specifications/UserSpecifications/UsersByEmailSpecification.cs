using Eventify.Domain.Common;
using Eventify.Domain.Entities.Users;

namespace Eventify.Domain.Specifications.UserSpecifications
{
    public class UsersByEmailSpecification : BaseSpecification<User>
    {
        public UsersByEmailSpecification(string email)
            : base(u => u.Email == email.ToLowerInvariant())
        {
            AddInclude(u => u.UserRoles);
            AddInclude(u => u.Preferences);
        }
    }
}
