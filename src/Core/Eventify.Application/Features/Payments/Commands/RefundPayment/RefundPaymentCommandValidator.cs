using FluentValidation;

namespace Eventify.Application.Features.Payments.Commands.RefundPayment
{
    public class RefundPaymentCommandValidator : AbstractValidator<RefundPaymentCommand>
    {
        public RefundPaymentCommandValidator()
        {
            RuleFor(x => x.PaymentId)
                .NotEmpty()
                .WithMessage("Payment ID is required");

            RuleFor(x => x.Reason)
                .NotEmpty()
                .WithMessage("Refund reason is required")
                .Must(reason => new[] { "duplicate", "fraudulent", "requested_by_customer" }.Contains(reason))
                .WithMessage("Invalid refund reason");

            RuleFor(x => x.Amount.Amount)
                .GreaterThan(0)
                .When(x => x.Amount != null)
                .WithMessage("Refund amount must be greater than zero");
        }
    }
}
