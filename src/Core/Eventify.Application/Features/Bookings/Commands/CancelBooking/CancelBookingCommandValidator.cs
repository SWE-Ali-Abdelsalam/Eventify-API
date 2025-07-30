using FluentValidation;

namespace Eventify.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
    {
        public CancelBookingCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty()
                .WithMessage("Booking ID is required");

            RuleFor(x => x.RequestedBy)
                .NotEmpty()
                .WithMessage("Requestor information is required");

            RuleFor(x => x.CancellationReason)
                .MaximumLength(1000)
                .WithMessage("Cancellation reason cannot exceed 1000 characters");
        }
    }
}
