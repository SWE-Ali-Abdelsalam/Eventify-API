using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;

namespace Eventify.Domain.Entities.Supporting
{
    public class EventDocument : BaseEntity
    {
        public Guid EventId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public string FileUrl { get; private set; } = string.Empty;
        public string FileName { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long FileSize { get; private set; }
        public bool IsPublic { get; private set; } = true;
        public int DownloadCount { get; private set; }
        public DateTime? LastDownloadedAt { get; private set; }
        public string? UploadedBy { get; private set; }

        // Navigation properties
        public Event Event { get; private set; } = null!;

        private EventDocument() { } // EF Core

        public EventDocument(Guid eventId, string name, string description, string fileUrl,
                            string fileName, string contentType, long fileSize, bool isPublic = true)
        {
            EventId = eventId;
            Name = name;
            Description = description;
            FileUrl = fileUrl;
            FileName = fileName;
            ContentType = contentType;
            FileSize = fileSize;
            IsPublic = isPublic;
        }

        public void UpdateDetails(string name, string description, bool isPublic)
        {
            Name = name;
            Description = description;
            IsPublic = isPublic;
            UpdatedAt = DateTime.UtcNow;
        }

        public void IncrementDownloadCount()
        {
            DownloadCount++;
            LastDownloadedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
