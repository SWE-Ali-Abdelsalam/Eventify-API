using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using MediatR;

namespace Eventify.Application.Features.Authentication.Commands.Logout
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Result>
    {
        private readonly IAuthenticationService _authenticationService;

        public LogoutCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.LogoutAsync(request.UserId);
        }
    }
}
