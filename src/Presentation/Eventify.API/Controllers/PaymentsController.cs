using Asp.Versioning;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Payments.Commands.ConfirmPayment;
using Eventify.Application.Features.Payments.Commands.CreatePayment;
using Eventify.Application.Features.Payments.Commands.RefundPayment;
using Eventify.Application.Features.Payments.Queries.GetPayment;
using Eventify.Application.Features.Payments.Queries.GetUserPayments;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Eventify.API.Controllers
{
    /// <summary>
    /// Payment processing and management endpoints
    /// </summary>
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiVersion("1.0")]
    [Authorize(Policy = "ActiveUser")]
    [Produces("application/json")]
    public class PaymentsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public PaymentsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>
        /// Create a payment for a booking
        /// </summary>
        /// <param name="command">Payment details</param>
        /// <returns>Payment intent with client secret for frontend processing</returns>
        [HttpPost]
        [ProducesResponseType(typeof(object), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetPayment), new { id = result.Value.PaymentId }, new
                {
                    success = true,
                    data = result.Value,
                    message = "Payment created successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        /// <summary>
        /// Confirm a payment after client-side processing
        /// </summary>
        /// <param name="command">Payment confirmation details</param>
        /// <returns>Payment confirmation result</returns>
        [HttpPost("confirm")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> ConfirmPayment([FromBody] ConfirmPaymentCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Value,
                    message = result.Value.IsSuccessful ? "Payment completed successfully" : "Payment requires additional action"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        /// <summary>
        /// Get payment details by ID
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns>Payment details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPayment(Guid id)
        {
            var query = new GetPaymentQuery { PaymentId = id };
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

        /// <summary>
        /// Get current user's payments
        /// </summary>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>List of user's payments</returns>
        [HttpGet("my-payments")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> GetMyPayments([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetUserPaymentsQuery
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Value,
                    pagination = new
                    {
                        pageNumber,
                        pageSize,
                        totalItems = result.Value.Count
                    }
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        /// <summary>
        /// Process a refund for a payment (Admin or booking owner only)
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <param name="command">Refund details</param>
        /// <returns>Refund result</returns>
        [HttpPost("{id}/refund")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> RefundPayment(Guid id, [FromBody] RefundPaymentCommand command)
        {
            command.PaymentId = id;
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Value,
                    message = "Refund processed successfully"
                });
            }

            return BadRequest(new
            {
                success = false,
                message = result.Error
            });
        }

        /// <summary>
        /// Get user payments (Admin only - can specify any user)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>User's payments</returns>
        [HttpGet("users/{userId}")]
        [Authorize(Policy = "AdminOnly")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUserPayments(Guid userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var query = new GetUserPaymentsQuery
            {
                UserId = userId,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
            {
                return Ok(new
                {
                    success = true,
                    data = result.Value,
                    pagination = new
                    {
                        pageNumber,
                        pageSize,
                        totalItems = result.Value.Count
                    }
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
