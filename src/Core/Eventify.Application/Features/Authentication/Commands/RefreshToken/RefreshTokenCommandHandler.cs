using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Common.Models;
using MediatR;

namespace Eventify.Application.Features.Authentication.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResult>>
    {
        private readonly IAuthenticationService _authenticationService;

        public RefreshTokenCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.RefreshTokenAsync(request.AccessToken, request.RefreshToken, request.IpAddress);
        }
    }
}
