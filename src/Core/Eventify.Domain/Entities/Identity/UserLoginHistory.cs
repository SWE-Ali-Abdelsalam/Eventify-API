using Eventify.Domain.Common;

namespace Eventify.Domain.Entities.Identity
{
    public class UserLoginHistory : BaseEntity
    {
        public Guid UserId { get; set; }
        public DateTime LoginTime { get; set; } = DateTime.UtcNow;
        public string? IpAddress { get; set; }
        public string? UserAgent { get; set; }
        public bool IsSuccessful { get; set; }
        public string? FailureReason { get; set; }
        public string? Location { get; set; }
        public string? Device { get; set; }

        // Navigation properties
        public ApplicationUser User { get; set; } = null!;

        public UserLoginHistory() { }

        public UserLoginHistory(Guid userId, string? ipAddress, string? userAgent, bool isSuccessful, string? failureReason = null)
        {
            UserId = userId;
            IpAddress = ipAddress;
            UserAgent = userAgent;
            IsSuccessful = isSuccessful;
            FailureReason = failureReason;
        }
    }
}
