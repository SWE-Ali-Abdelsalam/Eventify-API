using FluentValidation;

namespace Eventify.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
    {
        public UpdateUserProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage("First name is required")
                .MaximumLength(100)
                .WithMessage("First name must not exceed 100 characters");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage("Last name is required")
                .MaximumLength(100)
                .WithMessage("Last name must not exceed 100 characters");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(20)
                .WithMessage("Phone number must not exceed 20 characters")
                .Matches(@"^\+?[\d\s\-\(\)]+$")
                .WithMessage("Invalid phone number format")
                .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

            RuleFor(x => x.Bio)
                .MaximumLength(1000)
                .WithMessage("Bio must not exceed 1000 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Bio));

            RuleFor(x => x.Website)
                .MaximumLength(255)
                .WithMessage("Website URL must not exceed 255 characters")
                .Must(BeAValidUrl)
                .WithMessage("Invalid website URL format")
                .When(x => !string.IsNullOrWhiteSpace(x.Website));

            RuleFor(x => x.Company)
                .MaximumLength(255)
                .WithMessage("Company name must not exceed 255 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Company));

            RuleFor(x => x.JobTitle)
                .MaximumLength(255)
                .WithMessage("Job title must not exceed 255 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.JobTitle));

            RuleFor(x => x.DateOfBirth)
                .LessThan(DateTime.Today)
                .WithMessage("Date of birth must be in the past")
                .GreaterThan(DateTime.Today.AddYears(-120))
                .WithMessage("Date of birth is not valid")
                .When(x => x.DateOfBirth.HasValue);

            RuleFor(x => x.TimeZone)
                .MaximumLength(50)
                .WithMessage("Time zone must not exceed 50 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.TimeZone));

            RuleFor(x => x.Language)
                .MaximumLength(10)
                .WithMessage("Language code must not exceed 10 characters")
                .When(x => !string.IsNullOrWhiteSpace(x.Language));

            // Address validation
            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address!.Street)
                    .NotEmpty()
                    .WithMessage("Street address is required when address is provided")
                    .MaximumLength(255)
                    .WithMessage("Street address must not exceed 255 characters");

                RuleFor(x => x.Address!.City)
                    .NotEmpty()
                    .WithMessage("City is required when address is provided")
                    .MaximumLength(100)
                    .WithMessage("City must not exceed 100 characters");

                RuleFor(x => x.Address!.State)
                    .NotEmpty()
                    .WithMessage("State is required when address is provided")
                    .MaximumLength(100)
                    .WithMessage("State must not exceed 100 characters");

                RuleFor(x => x.Address!.Country)
                    .NotEmpty()
                    .WithMessage("Country is required when address is provided")
                    .MaximumLength(100)
                    .WithMessage("Country must not exceed 100 characters");

                RuleFor(x => x.Address!.PostalCode)
                    .NotEmpty()
                    .WithMessage("Postal code is required when address is provided")
                    .MaximumLength(20)
                    .WithMessage("Postal code must not exceed 20 characters");

                RuleFor(x => x.Address!.Latitude)
                    .InclusiveBetween(-90, 90)
                    .WithMessage("Latitude must be between -90 and 90 degrees")
                    .When(x => x.Address!.Latitude.HasValue);

                RuleFor(x => x.Address!.Longitude)
                    .InclusiveBetween(-180, 180)
                    .WithMessage("Longitude must be between -180 and 180 degrees")
                    .When(x => x.Address!.Longitude.HasValue);
            });
        }

        private static bool BeAValidUrl(string? url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return true;

            return Uri.TryCreate(url, UriKind.Absolute, out var result) &&
                   (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
