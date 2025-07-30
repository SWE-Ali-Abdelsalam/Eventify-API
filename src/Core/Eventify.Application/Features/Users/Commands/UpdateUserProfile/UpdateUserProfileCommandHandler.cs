using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Users;
using Eventify.Domain.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eventify.Application.Features.Users.Commands.UpdateUserProfile
{
    public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, Result>
    {
        private readonly IRepository<User> _userRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

        public UpdateUserProfileCommandHandler(
            IRepository<User> userRepository,
            ICurrentUserService currentUserService,
            ILogger<UpdateUserProfileCommandHandler> logger)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Security check: Users can only update their own profile unless they're admin
                if (request.UserId.ToString() != _currentUserService.UserId &&
                    !_currentUserService.IsInRole("Admin"))
                {
                    return Result.Failure("You don't have permission to update this profile");
                }

                var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                // Update basic profile information
                user.UpdateProfile(
                    request.FirstName,
                    request.LastName,
                    request.PhoneNumber,
                    request.DateOfBirth,
                    request.Bio,
                    request.Website,
                    request.Company,
                    request.JobTitle
                );

                // Update address if provided
                if (request.Address != null)
                {
                    var address = new Address(
                        request.Address.Street,
                        request.Address.City,
                        request.Address.State,
                        request.Address.Country,
                        request.Address.PostalCode,
                        request.Address.Latitude,
                        request.Address.Longitude
                    );

                    user.UpdateAddress(address);
                }

                // Update timezone and language if provided
                if (!string.IsNullOrWhiteSpace(request.TimeZone))
                {
                    // You might want to validate timezone here
                    user.UpdateTimeZone(request.TimeZone);
                }

                if (!string.IsNullOrWhiteSpace(request.Language))
                {
                    user.UpdateLanguage(request.Language);
                }

                await _userRepository.UpdateAsync(user, cancellationToken);

                _logger.LogInformation("User profile updated successfully: {UserId}", request.UserId);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile: {UserId}", request.UserId);
                return Result.Failure("An error occurred while updating the profile");
            }
        }
    }
}
