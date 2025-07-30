using Eventify.Application.Common;
using Eventify.Application.Features.Payments.DTOs;

namespace Eventify.Application.Features.Payments.Commands.ConfirmPayment
{
    public class ConfirmPaymentCommand : BaseCommand<Result<ConfirmPaymentResult>>
    {
        public string PaymentIntentId { get; set; } = string.Empty;
        public string? PaymentMethodId { get; set; }
    }
}
