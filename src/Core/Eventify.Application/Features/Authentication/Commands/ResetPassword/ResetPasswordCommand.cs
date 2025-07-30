using Eventify.Application.Common;

namespace Eventify.Application.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommand : BaseCommand<Result>
    {
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
