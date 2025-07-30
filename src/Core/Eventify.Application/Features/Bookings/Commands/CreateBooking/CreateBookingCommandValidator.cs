using FluentValidation;

namespace Eventify.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
    {
        public CreateBookingCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required");

            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");

            RuleFor(x => x.TicketSelections)
                .NotEmpty()
                .WithMessage("At least one ticket selection is required");

            RuleForEach(x => x.TicketSelections).SetValidator(new TicketSelectionValidator());

            RuleFor(x => x.AttendeeInformation)
                .NotEmpty()
                .WithMessage("Attendee information is required");

            RuleForEach(x => x.AttendeeInformation).SetValidator(new AttendeeInfoValidator());

            RuleFor(x => x.SpecialRequests)
                .MaximumLength(2000)
                .WithMessage("Special requests cannot exceed 2000 characters");
        }
    }
}
