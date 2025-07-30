using Asp.Versioning;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Users.Commands.UpdateUserProfile;
using Eventify.Application.Features.Users.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventify.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(Policy = "ActiveUser")]
    [Produces("application/json")]
    public class UserProfileController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public UserProfileController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetProfile()
        {
            var query = new GetUserProfileQuery
            {
                UserId = Guid.Parse(_currentUserService.UserId!)
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Value
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPut]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileCommand command)
        {
            command.UserId = Guid.Parse(_currentUserService.UserId!);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Profile updated successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpGet("{userId}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserProfile(Guid userId)
        {
            var query = new GetUserProfileQuery
            {
                UserId = userId
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Value
                });
            }

            return NotFound(new
            {
                success = false,
                message = result.Error
            });
        }
    }
}
