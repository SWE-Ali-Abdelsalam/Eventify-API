using Eventify.Application.Common;
using Eventify.Application.Common.Models;

namespace Eventify.Application.Features.Authentication.Commands.Login
{
    public class LoginCommand : BaseCommand<Result<AuthenticationResult>>
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
        public bool RememberMe { get; set; }
    }
}
