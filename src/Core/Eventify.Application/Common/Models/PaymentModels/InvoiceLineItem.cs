using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Common.Models.PaymentModels
{
    public class InvoiceLineItem
    {
        public string Description { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public Money UnitPrice { get; set; } = null!;
        public Money TotalPrice { get; set; } = null!;
    }
}
