using FluentValidation;

namespace Eventify.Application.Features.Payments.Commands.ConfirmPayment
{
    public class ConfirmPaymentCommandValidator : AbstractValidator<ConfirmPaymentCommand>
    {
        public ConfirmPaymentCommandValidator()
        {
            RuleFor(x => x.PaymentIntentId)
                .NotEmpty()
                .WithMessage("Payment intent ID is required");
        }
    }
}
