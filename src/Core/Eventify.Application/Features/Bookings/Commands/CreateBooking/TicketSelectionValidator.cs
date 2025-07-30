using Eventify.Application.Features.Bookings.DTOs;
using FluentValidation;

namespace Eventify.Application.Features.Bookings.Commands.CreateBooking
{
    public class TicketSelectionValidator : AbstractValidator<TicketSelection>
    {
        public TicketSelectionValidator()
        {
            RuleFor(x => x.TicketTypeId)
                .NotEmpty()
                .WithMessage("Ticket type ID is required");

            RuleFor(x => x.Quantity)
                .GreaterThan(0)
                .WithMessage("Quantity must be greater than 0")
                .LessThanOrEqualTo(10)
                .WithMessage("Maximum 10 tickets per type allowed");
        }
    }
}
