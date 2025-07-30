using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Entities.Tickets
{
    public class TicketType : BaseEntity
    {
        public Guid EventId { get; private set; }
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Money Price { get; private set; } = null!;
        public int Quantity { get; private set; }
        public int SoldQuantity { get; private set; }
        public DateTime? SaleStartDate { get; private set; }
        public DateTime? SaleEndDate { get; private set; }
        public int? MaxPerOrder { get; private set; }
        public int? MinPerOrder { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool IsVisible { get; private set; } = true;
        public string? Terms { get; private set; }
        public int SortOrder { get; private set; }

        // Navigation properties
        public Event Event { get; private set; } = null!;
        public ICollection<BookingTicket> BookingTickets { get; private set; } = new List<BookingTicket>();

        private TicketType() { } // EF Core

        public TicketType(Guid eventId, string name, string description, Money price, int quantity)
        {
            EventId = eventId;
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            SoldQuantity = 0;
            MinPerOrder = 1;
        }

        public void UpdateDetails(string name, string description, Money price, int quantity,
                                 int? maxPerOrder, int? minPerOrder, string? terms)
        {
            Name = name;
            Description = description;
            Price = price;
            Quantity = quantity;
            MaxPerOrder = maxPerOrder;
            MinPerOrder = minPerOrder;
            Terms = terms;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetSalePeriod(DateTime? startDate, DateTime? endDate)
        {
            SaleStartDate = startDate;
            SaleEndDate = endDate;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetVisibility(bool isVisible)
        {
            IsVisible = isVisible;
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

        public void IncrementSold(int quantity)
        {
            if (SoldQuantity + quantity > Quantity)
                throw new InvalidOperationException("Not enough tickets available");

            SoldQuantity += quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void DecrementSold(int quantity)
        {
            if (SoldQuantity - quantity < 0)
                throw new InvalidOperationException("Cannot reduce sold quantity below zero");

            SoldQuantity -= quantity;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateSortOrder(int sortOrder)
        {
            SortOrder = sortOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public int AvailableQuantity => Quantity - SoldQuantity;
        public bool IsAvailable => IsActive && AvailableQuantity > 0 && IsOnSale;
        public bool IsOnSale => (!SaleStartDate.HasValue || DateTime.UtcNow >= SaleStartDate) &&
                               (!SaleEndDate.HasValue || DateTime.UtcNow <= SaleEndDate);
        public bool IsSoldOut => SoldQuantity >= Quantity;
        public decimal SoldPercentage => Quantity > 0 ? (decimal)SoldQuantity / Quantity * 100 : 0;
    }
}
