using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using MediatR;

namespace Eventify.Application.Features.Authentication.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, Result>
    {
        private readonly IAuthenticationService _authenticationService;

        public ForgotPasswordCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.ForgotPasswordAsync(request.Email);
        }
    }
}
