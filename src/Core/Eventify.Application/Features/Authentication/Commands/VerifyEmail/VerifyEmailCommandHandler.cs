using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using MediatR;

namespace Eventify.Application.Features.Authentication.Commands.VerifyEmail
{
    public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, Result>
    {
        private readonly IAuthenticationService _authenticationService;

        public VerifyEmailCommandHandler(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public async Task<Result> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
        {
            return await _authenticationService.VerifyEmailAsync(request.Email, request.Token);
        }
    }
}
