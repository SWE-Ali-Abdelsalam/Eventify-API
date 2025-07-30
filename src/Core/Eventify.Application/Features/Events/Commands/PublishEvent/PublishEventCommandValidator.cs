using FluentValidation;

namespace Eventify.Application.Features.Events.Commands.PublishEvent
{
    public class PublishEventCommandValidator : AbstractValidator<PublishEventCommand>
    {
        public PublishEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required");
        }
    }
}
