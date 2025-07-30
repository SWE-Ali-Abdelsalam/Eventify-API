namespace Eventify.Application.Features.Events.DTOs
{
    public class TicketTypeSummaryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int AvailableQuantity { get; set; }
        public bool IsAvailable { get; set; }
    }
}
