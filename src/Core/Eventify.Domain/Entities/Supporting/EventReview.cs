using Eventify.Domain.Common;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.Entities.Users;

namespace Eventify.Domain.Entities.Supporting
{
    public class EventReview : BaseEntity
    {
        public Guid EventId { get; private set; }
        public Guid UserId { get; private set; }
        public int Rating { get; private set; } // 1-5 stars
        public string? Title { get; private set; }
        public string? Comment { get; private set; }
        public bool IsApproved { get; private set; }
        public DateTime? ApprovalDate { get; private set; }
        public string? ApprovedBy { get; private set; }
        public bool IsAnonymous { get; private set; }

        // Navigation properties
        public Event Event { get; private set; } = null!;
        public User User { get; private set; } = null!;

        private EventReview() { } // EF Core

        public EventReview(Guid eventId, Guid userId, int rating, string? title = null,
                          string? comment = null, bool isAnonymous = false)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            EventId = eventId;
            UserId = userId;
            Rating = rating;
            Title = title;
            Comment = comment;
            IsAnonymous = isAnonymous;
        }

        public void UpdateReview(int rating, string? title, string? comment)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            Rating = rating;
            Title = title;
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Approve(string approvedBy)
        {
            IsApproved = true;
            ApprovalDate = DateTime.UtcNow;
            ApprovedBy = approvedBy;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
