using Asp.Versioning;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Events.Commands.CreateEvent;
using Eventify.Application.Features.Events.Commands.DeleteEvent;
using Eventify.Application.Features.Events.Commands.PublishEvent;
using Eventify.Application.Features.Events.Commands.UpdateEvent;
using Eventify.Application.Features.Events.Queries.GetEvent;
using Eventify.Application.Features.Events.Queries.GetEvents;
using Eventify.Application.Features.Events.Queries.GetEventsByOrganizer;
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
    public class EventsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public EventsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        public async Task<IActionResult> GetEvents([FromQuery] GetEventsQuery query)
        {
            // If user is not authenticated, only show public events
            if (!_currentUserService.IsAuthenticated)
            {
                query.PublicOnly = true;
            }
            // If user is participant, only show public events
            else if (_currentUserService.IsInRole("Participant") &&
                    !_currentUserService.IsInRole("Organizer") &&
                    !_currentUserService.IsInRole("Admin"))
            {
                query.PublicOnly = true;
            }

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Value,
                    message = "Events retrieved successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetEvent(Guid id)
        {
            var query = new GetEventQuery { EventId = id };
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

        [HttpPost]
        [Authorize(Policy = "CanManageEvents")]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreateEvent([FromBody] CreateEventCommand command)
        {
            // Set the organizer ID from the current user
            command.OrganizerId = Guid.Parse(_currentUserService.UserId!);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetEvent), new { id = result.Value }, new
                {
                    success = true,
                    data = result.Value,
                    message = "Event created successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CanManageEvents")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateEvent(Guid id, [FromBody] UpdateEventCommand command)
        {
            command.EventId = id;
            command.RequestedBy = _currentUserService.UserId!;

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Event updated successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPost("{id}/publish")]
        [Authorize(Policy = "CanManageEvents")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PublishEvent(Guid id)
        {
            var command = new PublishEventCommand
            {
                EventId = id,
                RequestedBy = _currentUserService.UserId!
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Event published successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CanManageEvents")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteEvent(Guid id)
        {
            var command = new DeleteEventCommand
            {
                EventId = id,
                RequestedBy = _currentUserService.UserId!
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Event deleted successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpGet("my-events")]
        [Authorize(Policy = "CanManageEvents")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetMyEvents()
        {
            var query = new GetEventsByOrganizerQuery
            {
                OrganizerId = Guid.Parse(_currentUserService.UserId!)
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
    }
}
