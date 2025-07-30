namespace Eventify.Application.Features.Admin.DTOs
{
    public class UserManagementDto
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool IsPhoneVerified { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public List<UserRoleDto> Roles { get; set; } = new();
        public int TotalBookings { get; set; }
        public int TotalEvents { get; set; }
        public decimal TotalSpent { get; set; }
    }
}
