namespace Eventify.Application.Features.Users.DTOs
{
    public class UserProfileDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Bio { get; set; }
        public string? Website { get; set; }
        public string? Company { get; set; }
        public string? JobTitle { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string? TimeZone { get; set; }
        public string? Language { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public AddressDto? Address { get; set; }
        public List<string> Roles { get; set; } = new();

        public string FullName => $"{FirstName} {LastName}";
    }
}
