namespace Eventify.Application.Features.Bookings.DTOs
{
    public class EventSummaryDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsVirtual { get; set; }
        public string? VirtualMeetingUrl { get; set; }
    }
}
