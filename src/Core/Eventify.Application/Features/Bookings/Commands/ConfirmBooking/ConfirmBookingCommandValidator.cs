using FluentValidation;

namespace Eventify.Application.Features.Bookings.Commands.ConfirmBooking
{
    public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
    {
        public ConfirmBookingCommandValidator()
        {
            RuleFor(x => x.BookingId)
                .NotEmpty()
                .WithMessage("Booking ID is required");

            RuleFor(x => x.ApprovedBy)
                .NotEmpty()
                .WithMessage("Approver information is required");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters");
        }
    }
}
