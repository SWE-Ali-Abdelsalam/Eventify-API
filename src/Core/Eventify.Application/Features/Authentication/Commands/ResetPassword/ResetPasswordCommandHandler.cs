using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using MediatR;

namespace Eventify.Application.Features.Authentication.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, Result>
    {
        private readonly IAuthenticationService _authenticationService;

        public ResetPasswordCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.ResetPasswordAsync(request.Email, request.Token, request.NewPassword);
        }
    }
}
