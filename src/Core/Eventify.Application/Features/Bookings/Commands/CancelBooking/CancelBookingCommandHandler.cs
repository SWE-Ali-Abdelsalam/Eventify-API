using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Specifications.BookingSpecifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eventify.Application.Features.Bookings.Commands.CancelBooking
{
    public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, Result>
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CancelBookingCommandHandler> _logger;

        public CancelBookingCommandHandler(
            IRepository<Booking> bookingRepository,
            ICurrentUserService currentUserService,
            ILogger<CancelBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bookingSpec = new BookingWithDetailsSpecification(request.BookingId);
                var booking = await _bookingRepository.GetBySpecAsync(bookingSpec, cancellationToken);

                if (booking == null)
                {
                    return Result.Failure("Booking not found");
                }

                // Check if user has permission to cancel this booking
                if (booking.UserId.ToString() != request.RequestedBy &&
                    !_currentUserService.IsInRole("Admin") &&
                    !_currentUserService.IsInRole("Organizer"))
                {
                    return Result.Failure("You don't have permission to cancel this booking");
                }

                // Check if booking can be cancelled
                if (!booking.CanBeCancelled)
                {
                    return Result.Failure("This booking cannot be cancelled");
                }

                // Check cancellation policy (e.g., must cancel X hours before event)
                var hoursUntilEvent = (booking.Event.EventDates.StartDate - DateTime.UtcNow).TotalHours;
                if (hoursUntilEvent < 24 && !_currentUserService.IsInRole("Admin"))
                {
                    return Result.Failure("Bookings can only be cancelled up to 24 hours before the event");
                }

                // Cancel the booking
                booking.Cancel(request.CancellationReason ?? "Cancelled by user");

                // Restore event capacity
                booking.Event.DecrementRegistrations(booking.TotalTickets);

                // Restore ticket availability
                foreach (var ticket in booking.Tickets)
                {
                    ticket.TicketType.DecrementSold(1);
                }

                await _bookingRepository.UpdateAsync(booking, cancellationToken);

                _logger.LogInformation("Booking cancelled: {BookingId} by user: {UserId}",
                    booking.Id, request.RequestedBy);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling booking: {BookingId}", request.BookingId);
                return Result.Failure("An error occurred while cancelling the booking");
            }
        }
    }
}
