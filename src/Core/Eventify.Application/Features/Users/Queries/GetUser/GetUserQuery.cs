using Eventify.Application.Common;
using Eventify.Application.Features.Users.DTOs;

namespace Eventify.Application.Features.Users.Queries.GetUser
{
    public class GetUserQuery : BaseQuery<Result<UserDto>>
    {
        public Guid UserId { get; set; }
    }
}
