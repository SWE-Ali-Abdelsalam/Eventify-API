using Eventify.Domain.Common;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Entities.Payments
{
    public class PaymentRefund : BaseEntity
    {
        public Guid PaymentId { get; private set; }
        public Money Amount { get; private set; } = null!;
        public string Reason { get; private set; } = string.Empty;
        public string? ExternalRefundId { get; private set; }
        public DateTime RefundedAt { get; private set; } = DateTime.UtcNow;
        public string? ProcessorResponse { get; private set; }

        // Navigation properties
        public Payment Payment { get; private set; } = null!;

        private PaymentRefund() { } // EF Core

        public PaymentRefund(Guid paymentId, Money amount, string reason)
        {
            PaymentId = paymentId;
            Amount = amount;
            Reason = reason;
        }

        public void SetExternalRefundId(string externalRefundId)
        {
            ExternalRefundId = externalRefundId;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetProcessorResponse(string processorResponse)
        {
            ProcessorResponse = processorResponse;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
