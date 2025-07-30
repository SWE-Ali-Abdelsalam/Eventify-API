using Eventify.Application.Common;
using Eventify.Application.Features.Users.DTOs;

namespace Eventify.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommand : BaseCommand<Result>
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }
        public string? Website { get; set; }
        public string? Company { get; set; }
        public string? JobTitle { get; set; }
        public string? TimeZone { get; set; }
        public string? Language { get; set; }
        public AddressUpdateDto? Address { get; set; }
    }
}
