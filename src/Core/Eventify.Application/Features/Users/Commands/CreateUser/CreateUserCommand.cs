using Eventify.Application.Common;

namespace Eventify.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommand : BaseCommand<Result<Guid>>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
    }
}
