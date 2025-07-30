using FluentValidation;

namespace Eventify.Application.Features.Payments.Commands.CreatePayment
{
    public class CreatePaymentCommandValidator : AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty()
                .WithMessage("Booking ID is required");

            RuleFor(x => x.PaymentMethodId)
                .NotEmpty()
                .WithMessage("Payment method is required");

            RuleFor(x => x.ReceiptEmail)
                .EmailAddress()
                .When(x => !string.IsNullOrEmpty(x.ReceiptEmail))
                .WithMessage("Invalid email format");
        }
    }
}
