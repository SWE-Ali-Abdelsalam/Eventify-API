namespace Eventify.Application.Common.Models.PaymentModels
{
    public class CreateCustomerRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public BillingDetails? BillingDetails { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
