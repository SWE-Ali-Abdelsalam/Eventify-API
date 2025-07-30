using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Common.Models;
using MediatR;

namespace Eventify.Application.Features.Authentication.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResult>>
    {
        private readonly IAuthenticationService _authenticationService;

        public LoginCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.LoginAsync(request.Email, request.Password, request.IpAddress);
        }
    }
}
