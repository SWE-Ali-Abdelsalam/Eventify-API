using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using MediatR;

namespace Eventify.Application.Features.Authentication.Commands.ChangePassword
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, Result>
    {
        private readonly IAuthenticationService _authenticationService;

        public ChangePasswordCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.ChangePasswordAsync(request.UserId, request.CurrentPassword, request.NewPassword);
        }
    }
}
