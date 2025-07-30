namespace Eventify.Application.Common.Models.PaymentModels
{
    public class CustomerResult
    {
        public string CustomerId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public DateTime CreatedAt { get; set; }
        public BillingDetails? BillingDetails { get; set; }
    }
}
