using Eventify.Domain.Common;

namespace Eventify.Domain.Entities.Events
{
    public class EventCategory : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string? IconUrl { get; private set; }
        public string? Color { get; private set; }
        public bool IsActive { get; private set; } = true;
        public int SortOrder { get; private set; }

        // Navigation properties
        public ICollection<Event> Events { get; private set; } = new List<Event>();

        private EventCategory() { } // EF Core

        public EventCategory(string name, string description, string? iconUrl = null, string? color = null)
        {
            Name = name;
            Description = description;
            IconUrl = iconUrl;
            Color = color;
        }

        public void UpdateDetails(string name, string description, string? iconUrl, string? color)
        {
            Name = name;
            Description = description;
            IconUrl = iconUrl;
            Color = color;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateSortOrder(int sortOrder)
        {
            SortOrder = sortOrder;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
