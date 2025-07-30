using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Users;
using Eventify.Domain.Specifications.UserSpecifications;
using MediatR;

namespace Eventify.Application.Features.Admin.Commands.ManageUserRoles
{
    public class ManageUserRolesCommandHandler : IRequestHandler<ManageUserRolesCommand, Result>
    {
        private readonly IRepository<User> _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public ManageUserRolesCommandHandler(
            IRepository<User> userRepository,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(ManageUserRolesCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get user with roles
                var userSpec = new UserWithRolesSpecification(request.UserId);
                var user = await _userRepository.GetBySpecAsync(userSpec, cancellationToken);

                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                var requestedBy = request.RequestedBy ?? _currentUserService.UserId;

                // Process each role assignment
                foreach (var roleAssignment in request.RoleAssignments)
                {
                    if (roleAssignment.Assign)
                    {
                        // Assign role
                        user.AddRole(
                            roleAssignment.Role
                        );
                    }
                    else
                    {
                        // Remove role
                        user.RemoveRole(roleAssignment.Role);
                    }
                }

                await _userRepository.UpdateAsync(user, cancellationToken);

                return Result.Success();
            }
            catch (Exception ex)
            {
                return Result.Failure($"Error managing user roles: {ex.Message}");
            }
        }
    }
}
