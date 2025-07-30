using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Specifications.BookingSpecifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eventify.Application.Features.Bookings.Commands.ConfirmBooking
{
    public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, Result>
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IEmailService _emailService;
        private readonly ILogger<ConfirmBookingCommandHandler> _logger;

        public ConfirmBookingCommandHandler(
            IRepository<Booking> bookingRepository,
            ICurrentUserService currentUserService,
            IEmailService emailService,
            ILogger<ConfirmBookingCommandHandler> logger)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<Result> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var bookingSpec = new BookingWithDetailsSpecification(request.BookingId);
                var booking = await _bookingRepository.GetBySpecAsync(bookingSpec, cancellationToken);

                if (booking == null)
                {
                    return Result.Failure("Booking not found");
                }

                // Check if user has permission to confirm bookings for this event
                var hasPermission = _currentUserService.IsInRole("Admin") ||
                                   (_currentUserService.IsInRole("Organizer") && booking.Event.OrganizerId.ToString() == _currentUserService.UserId);

                if (!hasPermission)
                {
                    return Result.Failure("You don't have permission to confirm this booking");
                }

                // Check if booking can be confirmed
                if (booking.Status != BookingStatus.Pending)
                {
                    return Result.Failure("Only pending bookings can be confirmed");
                }

                // Confirm the booking
                booking.Approve(request.ApprovedBy);
                await _bookingRepository.UpdateAsync(booking, cancellationToken);

                // Send confirmation email
                await SendBookingConfirmationEmail(booking);

                _logger.LogInformation("Booking confirmed: {BookingId} by: {ApprovedBy}",
                    booking.Id, request.ApprovedBy);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming booking: {BookingId}", request.BookingId);
                return Result.Failure("An error occurred while confirming the booking");
            }
        }

        private async Task SendBookingConfirmationEmail(Booking booking)
        {
            try
            {
                var subject = $"Booking Confirmed - {booking.Event.Title}";
                var body = $@"
                <html>
                <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                    <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                        <h2 style='color: #28a745;'>Booking Confirmed!</h2>
                        <p>Dear {booking.User.FullName},</p>
                        <p>Your booking has been confirmed for the following event:</p>
                        
                        <div style='background-color: #f8f9fa; padding: 15px; border-radius: 4px; margin: 20px 0;'>
                            <h3>{booking.Event.Title}</h3>
                            <p><strong>Date:</strong> {booking.Event.EventDates.StartDate:yyyy-MM-dd HH:mm}</p>
                            <p><strong>Booking Number:</strong> {booking.BookingNumber}</p>
                            <p><strong>Total Tickets:</strong> {booking.TotalTickets}</p>
                            <p><strong>Check-in Code:</strong> {booking.CheckInCode}</p>
                        </div>
                        
                        <p>Please keep this confirmation email and your check-in code for event entry.</p>
                        
                        <p>We look forward to seeing you at the event!</p>
                        
                        <p>Best regards,<br>The Event Management Team</p>
                    </div>
                </body>
                </html>";

                await _emailService.SendEmailAsync(booking.User.Email, subject, body);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending booking confirmation email for booking: {BookingId}", booking.Id);
            }
        }
    }
}
