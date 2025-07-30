using Eventify.Application.Common;
using Eventify.Application.Common.Models;

namespace Eventify.Application.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommand : BaseCommand<Result<AuthenticationResult>>
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public string? IpAddress { get; set; }
    }
}
