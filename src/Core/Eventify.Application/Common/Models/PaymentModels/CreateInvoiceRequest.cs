using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Common.Models.PaymentModels
{
    public class CreateInvoiceRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        public Money Amount { get; set; } = null!;
        public string Currency { get; set; } = "USD";
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public List<InvoiceLineItem> LineItems { get; set; } = new();
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
