using Asp.Versioning;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Authentication.Commands.ChangePassword;
using Eventify.Application.Features.Authentication.Commands.ForgotPassword;
using Eventify.Application.Features.Authentication.Commands.Login;
using Eventify.Application.Features.Authentication.Commands.Logout;
using Eventify.Application.Features.Authentication.Commands.RefreshToken;
using Eventify.Application.Features.Authentication.Commands.Register;
using Eventify.Application.Features.Authentication.Commands.ResetPassword;
using Eventify.Application.Features.Authentication.Commands.VerifyEmail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventify.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Produces("application/json")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public AuthenticationController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            command.IpAddress = GetIpAddress();
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Value,
                    message = "Login successful"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(Login), new
                {
                    success = true,
                    data = result.Value,
                    message = "Registration successful. Please check your email for verification instructions."
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPost("refresh-token")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenCommand command)
        {
            command.IpAddress = GetIpAddress();
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Value,
                    message = "Token refreshed successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPost("forgot-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "If an account with that email exists, a password reset link has been sent."
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPost("reset-password")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Password has been reset successfully. You can now login with your new password."
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPost("verify-email")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Email has been verified successfully."
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPost("change-password")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            command.UserId = _currentUserService.UserId ?? string.Empty;
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Password has been changed successfully."
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPost("logout")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Logout()
        {
            var command = new LogoutCommand { UserId = _currentUserService.UserId ?? string.Empty };
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Logout successful."
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpGet("me")]
        [Authorize]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public Task<IActionResult> GetCurrentUser()
        {
            var userInfo = new
            {
                Id = _currentUserService.UserId,
                UserName = _currentUserService.UserName,
                IsAuthenticated = _currentUserService.IsAuthenticated,
                Roles = _currentUserService.GetRoles()
            };

            return Task.FromResult<IActionResult>(Ok(new
            {
                success = true,
                data = userInfo
            }));
        }

        [HttpGet("status")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GetAuthenticationStatus()
        {
            return Ok(new
            {
                success = true,
                data = new
                {
                    IsAuthenticated = _currentUserService.IsAuthenticated,
                    UserId = _currentUserService.UserId
                }
            });
        }

        private string GetIpAddress()
        {
            return Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].ToString().Split(',')[0].Trim()
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
