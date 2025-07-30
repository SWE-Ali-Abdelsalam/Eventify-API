using Eventify.Application.Common;

namespace Eventify.Application.Features.Authentication.Commands.VerifyEmail
{
    public class VerifyEmailCommand : BaseCommand<Result>
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
}
