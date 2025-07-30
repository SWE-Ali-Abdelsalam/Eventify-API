using Asp.Versioning;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Bookings.Commands.CancelBooking;
using Eventify.Application.Features.Bookings.Commands.ConfirmBooking;
using Eventify.Application.Features.Bookings.Commands.CreateBooking;
using Eventify.Application.Features.Bookings.Queries.GetBooking;
using Eventify.Application.Features.Bookings.Queries.GetUserBookings;
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
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public BookingsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpPost]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command)
        {
            // Set the user ID from the authenticated user
            command.UserId = Guid.Parse(_currentUserService.UserId!);

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetBooking), new { id = result.Value!.Id }, new
                {
                    success = true,
                    data = result.Value,
                    message = "Booking created successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetBooking(Guid id)
        {
            var query = new GetBookingQuery
            {
                BookingId = id,
                RequestedBy = _currentUserService.UserId!
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

        [HttpGet("my-bookings")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetMyBookings()
        {
            var query = new GetUserBookingsQuery
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

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> CancelBooking(Guid id, [FromBody] CancelBookingCommand command)
        {
            command.BookingId = id;
            command.RequestedBy = _currentUserService.UserId!;

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Booking cancelled successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        [HttpPost("{id}/confirm")]
        [Authorize(Policy = "CanManageEvents")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ConfirmBooking(Guid id)
        {
            var command = new ConfirmBookingCommand
            {
                BookingId = id,
                ApprovedBy = _currentUserService.UserId!
            };

            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    message = "Booking confirmed successfully"
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
