namespace Eventify.Application.Features.Events.DTOs
{
    public class EventDocumentDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
    }
}
