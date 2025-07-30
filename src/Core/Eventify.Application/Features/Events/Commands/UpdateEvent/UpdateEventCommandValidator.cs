using FluentValidation;

namespace Eventify.Application.Features.Events.Commands.UpdateEvent
{
    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator()
        {
            RuleFor(x => x.EventId)
                .NotEmpty()
                .WithMessage("Event ID is required");

            RuleFor(x => x.Title)
                .NotEmpty()
                .WithMessage("Event title is required")
                .MaximumLength(255)
                .WithMessage("Title must not exceed 255 characters");

            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("Event description is required")
                .MaximumLength(5000)
                .WithMessage("Description must not exceed 5000 characters");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required")
                .Must(date => date > DateTime.UtcNow)
                .WithMessage("Start date must be in the future");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("End date is required")
                .GreaterThan(x => x.StartDate)
                .WithMessage("End date must be after start date");

            RuleFor(x => x.MaxCapacity)
                .GreaterThan(0)
                .WithMessage("Maximum capacity must be greater than 0");

            RuleFor(x => x.VirtualMeetingUrl)
                .Must(BeAValidUrl)
                .WithMessage("Virtual meeting URL must be a valid URL")
                .When(x => x.IsVirtual && !string.IsNullOrEmpty(x.VirtualMeetingUrl));

            RuleFor(x => x.RegistrationOpenDate)
                .LessThan(x => x.StartDate)
                .WithMessage("Registration open date must be before event start date")
                .When(x => x.RegistrationOpenDate.HasValue);

            RuleFor(x => x.RegistrationCloseDate)
                .GreaterThan(x => x.RegistrationOpenDate)
                .WithMessage("Registration close date must be after registration open date")
                .LessThanOrEqualTo(x => x.StartDate)
                .WithMessage("Registration close date must be before or equal to event start date")
                .When(x => x.RegistrationCloseDate.HasValue);
        }

        private static bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrEmpty(url)) return true;
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
