using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Users.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Users;
using MediatR;

namespace Eventify.Application.Features.Users.Queries.GetUserProfile
{
    public class GetUserProfileQueryHandler : IRequestHandler<GetUserProfileQuery, Result<UserProfileDto>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetUserProfileQueryHandler(
            IRepository<User> userRepository,
            ICurrentUserService currentUserService)
        {
            _userRepository = userRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
        {
            // Security check: Users can only view their own profile unless they're admin
            if (request.UserId.ToString() != _currentUserService.UserId &&
                !_currentUserService.IsInRole("Admin"))
            {
                return Result<UserProfileDto>.Failure("You don't have permission to view this profile");
            }

            var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

            if (user == null)
            {
                return Result<UserProfileDto>.Failure("User not found");
            }

            var userProfileDto = new UserProfileDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImageUrl = user.ProfileImageUrl,
                DateOfBirth = user.DateOfBirth,
                Bio = user.Bio,
                Website = user.Website,
                Company = user.Company,
                JobTitle = user.JobTitle,
                IsEmailVerified = user.IsEmailVerified,
                IsPhoneVerified = user.IsPhoneVerified,
                TwoFactorEnabled = user.TwoFactorEnabled,
                TimeZone = user.TimeZone,
                Language = user.Language,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                LastLoginAt = user.LastLoginAt,
                Address = user.Address != null ? new AddressDto
                {
                    Street = user.Address.Street,
                    City = user.Address.City,
                    State = user.Address.State,
                    Country = user.Address.Country,
                    PostalCode = user.Address.PostalCode,
                    Latitude = user.Address.Latitude,
                    Longitude = user.Address.Longitude
                } : null,
                Roles = user.GetActiveRoleNames().ToList()
            };

            return Result<UserProfileDto>.Success(userProfileDto);
        }
    }
}
