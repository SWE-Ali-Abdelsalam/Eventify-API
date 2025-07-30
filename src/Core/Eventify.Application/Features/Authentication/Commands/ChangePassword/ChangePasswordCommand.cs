using Eventify.Application.Common;

namespace Eventify.Application.Features.Authentication.Commands.ChangePassword
{
    public class ChangePasswordCommand : BaseCommand<Result>
    {
        public string UserId { get; set; } = string.Empty;
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
