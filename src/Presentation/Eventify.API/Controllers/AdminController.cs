using Asp.Versioning;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Admin.Commands.ManageUserRoles;
using Eventify.Application.Features.Admin.Queries.GetSystemStats;
using Eventify.Application.Features.Admin.Queries.GetUserManagement;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventify.API.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(Policy = "AdminOnly")]
    [Produces("application/json")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public AdminController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("stats")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetSystemStats()
        {
            var query = new GetSystemStatsQuery();
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

        [HttpGet("users")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> GetUserManagement([FromQuery] GetUserManagementQuery query)
        {
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

        [HttpPost("users/roles")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> ManageUserRoles([FromBody] ManageUserRolesCommand command)
        {
            command.RequestedBy = _currentUserService.UserId!;

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "User roles updated successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }
    }
}
