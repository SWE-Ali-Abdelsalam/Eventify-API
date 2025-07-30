using Eventify.Application.Common;
using Eventify.Application.Features.Users.DTOs;

namespace Eventify.Application.Features.Users.Queries.GetUserProfile
{
    public class GetUserProfileQuery : BaseQuery<Result<UserProfileDto>>
    {
        public Guid UserId { get; set; }
    }
}
