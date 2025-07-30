namespace Eventify.Application.Features.Admin.DTOs
{
    public class UserRoleDto
    {
        public Guid Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; }
        public string? AssignedBy { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public bool IsValid { get; set; }
    }
}
