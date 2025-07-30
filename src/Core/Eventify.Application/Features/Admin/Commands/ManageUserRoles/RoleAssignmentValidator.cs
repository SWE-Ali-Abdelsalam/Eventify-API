using Eventify.Application.Features.Admin.DTOs;
using FluentValidation;

namespace Eventify.Application.Features.Admin.Commands.ManageUserRoles
{
    public class RoleAssignmentValidator : AbstractValidator<RoleAssignmentDto>
    {
        public RoleAssignmentValidator()
        {
            RuleFor(x => x.Role)
                .IsInEnum()
                .WithMessage("Invalid role specified");

            RuleFor(x => x.ExpiresAt)
                .GreaterThan(DateTime.UtcNow)
                .When(x => x.ExpiresAt.HasValue)
                .WithMessage("Expiration date must be in the future");

            RuleFor(x => x.Notes)
                .MaximumLength(1000)
                .WithMessage("Notes cannot exceed 1000 characters");
        }
    }
}
