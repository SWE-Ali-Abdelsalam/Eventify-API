namespace Eventify.Application.Features.Events.DTOs
{
    public class TicketTypeDetailDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public int SoldQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }
        public int? MaxPerOrder { get; set; }
        public int? MinPerOrder { get; set; }
        public bool IsAvailable { get; set; }
        public bool IsOnSale { get; set; }
        public string? Terms { get; set; }
    }
}
