using Eventify.Application.Common;
using Eventify.Application.Features.Payments.DTOs;
using Eventify.Domain.ValueObjects;

namespace Eventify.Application.Features.Payments.Commands.RefundPayment
{
    public class RefundPaymentCommand : BaseCommand<Result<RefundPaymentResult>>
    {
        public Guid PaymentId { get; set; }
        public Money? Amount { get; set; } // null for full refund
        public string Reason { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
