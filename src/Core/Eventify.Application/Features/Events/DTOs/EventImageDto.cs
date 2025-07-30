namespace Eventify.Application.Features.Events.DTOs
{
    public class EventImageDto
    {
        public Guid Id { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string? Caption { get; set; }
        public bool IsPrimary { get; set; }
    }
}
