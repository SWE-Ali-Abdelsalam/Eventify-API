using Eventify.Domain.Common;

namespace Eventify.Domain.Entities.Users
{
    public class UserPreference : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Key { get; private set; } = string.Empty;
        public string Value { get; private set; } = string.Empty;
        public string? Description { get; private set; }

        // Navigation properties
        public User User { get; private set; } = null!;

        private UserPreference() { } // EF Core

        public UserPreference(Guid userId, string key, string value, string? description = null)
        {
            UserId = userId;
            Key = key;
            Value = value;
            Description = description;
        }

        public void UpdateValue(string value)
        {
            Value = value;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
