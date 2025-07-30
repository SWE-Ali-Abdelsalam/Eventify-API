namespace Eventify.Application.Common.Models
{
    public class UserInfo
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
        public IDictionary<string, object> Claims { get; set; } = new Dictionary<string, object>();
    }
}
