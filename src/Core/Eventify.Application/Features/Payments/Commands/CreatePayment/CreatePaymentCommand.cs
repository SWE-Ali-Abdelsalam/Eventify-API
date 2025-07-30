using Eventify.Application.Common;
using Eventify.Application.Common.Models.PaymentModels;
using Eventify.Application.Features.Payments.DTOs;

namespace Eventify.Application.Features.Payments.Commands.CreatePayment
{
    public class CreatePaymentCommand : BaseCommand<Result<CreatePaymentResult>>
    {
        public Guid BookingId { get; set; }
        public string PaymentMethodId { get; set; } = string.Empty;
        public string? ReceiptEmail { get; set; }
        public BillingDetails? BillingDetails { get; set; }
        public bool SavePaymentMethod { get; set; }
    }
}
