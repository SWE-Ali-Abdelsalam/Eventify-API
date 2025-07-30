namespace Eventify.Application.Common.Models.PaymentModels
{
    public class UpdateCustomerRequest
    {
        public string? Email { get; set; }
        public string? Name { get; set; }
        public string? Phone { get; set; }
        public BillingDetails? BillingDetails { get; set; }
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
