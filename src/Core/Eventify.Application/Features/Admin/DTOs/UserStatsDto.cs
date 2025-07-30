namespace Eventify.Application.Features.Admin.DTOs
{
    public class UserStatsDto
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int NewUsersThisMonth { get; set; }
        public int VerifiedUsers { get; set; }
        public Dictionary<string, int> UsersByRole { get; set; } = new();
        public List<DailyUserRegistrationDto> DailyRegistrations { get; set; } = new();
    }
}
