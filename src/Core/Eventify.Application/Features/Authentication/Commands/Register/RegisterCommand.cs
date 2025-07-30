using Eventify.Application.Common;
using Eventify.Application.Common.Models;

namespace Eventify.Application.Features.Authentication.Commands.Register
{
    public class RegisterCommand : BaseCommand<Result<AuthenticationResult>>
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool AcceptTerms { get; set; }
    }
}
