namespace Eventify.Application.Features.Events.DTOs
{
    public class EventCategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Color { get; set; }
        public string? IconUrl { get; set; }
    }
}
