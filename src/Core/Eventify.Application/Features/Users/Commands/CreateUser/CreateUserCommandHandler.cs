using Eventify.Application.Common;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Users;
using MediatR;

namespace Eventify.Application.Features.Users.Commands.CreateUser
{
    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
    {
        private readonly IRepository<User> _userRepository;

        public CreateUserCommandHandler(IRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            // Check if user already exists
            var existingUser = await _userRepository.ExistsAsync(
                u => u.Email == request.Email.ToLowerInvariant(),
                cancellationToken);

            if (existingUser)
            {
                return Result<Guid>.Failure("User with this email already exists");
            }

            // Create new user
            var user = new User(request.FirstName, request.LastName, request.Email);

            if (!string.IsNullOrWhiteSpace(request.PhoneNumber))
            {
                user.UpdateProfile(request.FirstName, request.LastName, request.PhoneNumber, null, null, null, null, null);
            }

            var createdUser = await _userRepository.AddAsync(user, cancellationToken);

            return Result<Guid>.Success(createdUser.Id);
        }
    }
}
