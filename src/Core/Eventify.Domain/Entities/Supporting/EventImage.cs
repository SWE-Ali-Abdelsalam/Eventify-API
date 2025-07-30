using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;

namespace Eventify.Domain.Entities.Supporting
{
    public class EventImage : BaseEntity
    {
        public Guid EventId { get; private set; }
        public string ImageUrl { get; private set; } = string.Empty;
        public string? Caption { get; private set; }
        public string? AltText { get; private set; }
        public bool IsPrimary { get; private set; }
        public int SortOrder { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public long FileSize { get; private set; }
        public string? UploadedBy { get; private set; }

        // Navigation properties
        public Event Event { get; private set; } = null!;

        private EventImage() { } // EF Core

        public EventImage(Guid eventId, string imageUrl, string? caption = null,
                         string? altText = null, bool isPrimary = false, int width = 0,
                         int height = 0, long fileSize = 0)
        {
            EventId = eventId;
            ImageUrl = imageUrl;
            Caption = caption;
            AltText = altText;
            IsPrimary = isPrimary;
            Width = width;
            Height = height;
            FileSize = fileSize;
        }

        public void UpdateDetails(string? caption, string? altText, bool isPrimary, int sortOrder)
        {
            Caption = caption;
            AltText = altText;
            IsPrimary = isPrimary;
            SortOrder = sortOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetAsPrimary()
        {
            IsPrimary = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void RemoveAsPrimary()
        {
            IsPrimary = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
