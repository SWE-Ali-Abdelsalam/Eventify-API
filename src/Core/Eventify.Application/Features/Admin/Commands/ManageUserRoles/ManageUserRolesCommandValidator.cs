using FluentValidation;

namespace Eventify.Application.Features.Admin.Commands.ManageUserRoles
{
    public class ManageUserRolesCommandValidator : AbstractValidator<ManageUserRolesCommand>
    {
        public ManageUserRolesCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("User ID is required");

            RuleFor(x => x.RoleAssignments)
                .NotEmpty()
                .WithMessage("At least one role assignment is required");

            RuleForEach(x => x.RoleAssignments)
                .SetValidator(new RoleAssignmentValidator());
        }
    }
}
