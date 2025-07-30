using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Common.Models.PaymentModels
{
    public class ProcessRefundRequest
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public Money? Amount { get; set; } // null for full refund
        public string Reason { get; set; } = string.Empty;
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
