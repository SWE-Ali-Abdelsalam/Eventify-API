using Eventify.Application.Common;

namespace Eventify.Application.Features.Authentication.Commands.ForgotPassword
{
    public class ForgotPasswordCommand : BaseCommand<Result>
    {
        public string Email { get; set; } = string.Empty;
    }
}
