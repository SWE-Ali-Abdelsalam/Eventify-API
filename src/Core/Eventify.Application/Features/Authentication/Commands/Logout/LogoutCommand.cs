using Eventify.Application.Common;

namespace Eventify.Application.Features.Authentication.Commands.Logout
{
    public class LogoutCommand : BaseCommand<Result>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
