using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Bookings.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.Entities.Tickets;
using Eventify.Domain.Specifications.EventSpecifications;
using Eventify.Domain.ValueObjects;
using Eventify.Shared.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Eventify.Application.Features.Bookings.Commands.CreateBooking
{
    public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, Result<CreateBookingResult>>
    {
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<TicketType> _ticketTypeRepository;
        private readonly IRepository<Booking> _bookingRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ILogger<CreateBookingCommandHandler> _logger;

        public CreateBookingCommandHandler(
            IRepository<Event> eventRepository,
            IRepository<TicketType> ticketTypeRepository,
            IRepository<Booking> bookingRepository,
            ICurrentUserService currentUserService,
            ILogger<CreateBookingCommandHandler> logger)
        {
            _eventRepository = eventRepository;
            _ticketTypeRepository = ticketTypeRepository;
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
            _logger = logger;
        }

        public async Task<Result<CreateBookingResult>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // Get event with details
                var eventSpec = new EventWithDetailsSpecification(request.EventId);
                var eventEntity = await _eventRepository.GetBySpecAsync(eventSpec, cancellationToken);

                if (eventEntity == null)
                {
                    return Result<CreateBookingResult>.Failure("Event not found");
                }

                // Validate event is bookable
                if (eventEntity.Status != EventStatus.Published)
                {
                    return Result<CreateBookingResult>.Failure("Event is not available for booking");
                }

                if (!eventEntity.IsRegistrationOpen)
                {
                    return Result<CreateBookingResult>.Failure("Registration is not currently open for this event");
                }

                // Calculate total tickets and validate availability
                var totalTicketsRequested = request.TicketSelections.Sum(ts => ts.Quantity);

                if (totalTicketsRequested != request.AttendeeInformation.Count)
                {
                    return Result<CreateBookingResult>.Failure("Number of tickets must match number of attendees");
                }

                if (!eventEntity.HasAvailableSpots || eventEntity.AvailableSpots < totalTicketsRequested)
                {
                    return Result<CreateBookingResult>.Failure("Not enough tickets available");
                }

                // Validate and calculate ticket costs
                decimal totalAmount = 0;
                var ticketDetails = new List<(TicketType ticketType, int quantity)>();

                foreach (var selection in request.TicketSelections)
                {
                    var ticketType = eventEntity.TicketTypes.FirstOrDefault(tt => tt.Id == selection.TicketTypeId);
                    if (ticketType == null)
                    {
                        return Result<CreateBookingResult>.Failure($"Ticket type not found");
                    }

                    if (!ticketType.IsAvailable)
                    {
                        return Result<CreateBookingResult>.Failure($"Ticket type '{ticketType.Name}' is not available");
                    }

                    if (ticketType.AvailableQuantity < selection.Quantity)
                    {
                        return Result<CreateBookingResult>.Failure($"Not enough '{ticketType.Name}' tickets available");
                    }

                    // Check min/max per order constraints
                    if (ticketType.MinPerOrder.HasValue && selection.Quantity < ticketType.MinPerOrder)
                    {
                        return Result<CreateBookingResult>.Failure($"Minimum {ticketType.MinPerOrder} tickets required for '{ticketType.Name}'");
                    }

                    if (ticketType.MaxPerOrder.HasValue && selection.Quantity > ticketType.MaxPerOrder)
                    {
                        return Result<CreateBookingResult>.Failure($"Maximum {ticketType.MaxPerOrder} tickets allowed for '{ticketType.Name}'");
                    }

                    totalAmount += ticketType.Price.Amount * selection.Quantity;
                    ticketDetails.Add((ticketType, selection.Quantity));
                }

                // Create booking
                var totalMoney = new Money(totalAmount, eventEntity.TicketTypes.First().Price.Currency);
                var booking = new Booking(
                    request.EventId,
                    request.UserId,
                    totalMoney,
                    totalTicketsRequested,
                    request.IsGroupBooking,
                    request.PromoCode
                );

                booking.UpdateSpecialRequests(request.SpecialRequests);
                booking.UpdateAttendeeInformation(System.Text.Json.JsonSerializer.Serialize(request.AttendeeInformation));

                if (eventEntity.RequiresApproval)
                {
                    booking.SetRequiresApproval();
                }

                // Create booking tickets
                var attendeeIndex = 0;
                foreach (var (ticketType, quantity) in ticketDetails)
                {
                    for (int i = 0; i < quantity; i++)
                    {
                        var attendee = request.AttendeeInformation[attendeeIndex];
                        var bookingTicket = new BookingTicket(
                            booking.Id,
                            ticketType.Id,
                            attendee.FirstName,
                            attendee.LastName,
                            attendee.Email,
                            ticketType.Price
                        );

                        if (!string.IsNullOrEmpty(attendee.Phone))
                        {
                            bookingTicket.UpdateAttendeeInfo(attendee.FirstName, attendee.LastName, attendee.Email, attendee.Phone);
                        }

                        booking.Tickets.Add(bookingTicket);
                        attendeeIndex++;
                    }

                    // Update ticket type sold quantity
                    ticketType.IncrementSold(quantity);
                }

                // Update event capacity
                eventEntity.IncrementRegistrations(totalTicketsRequested);

                // Save booking
                var createdBooking = await _bookingRepository.AddAsync(booking, cancellationToken);

                // Auto-confirm if no approval required
                if (!eventEntity.RequiresApproval)
                {
                    createdBooking.Confirm();
                    await _bookingRepository.UpdateAsync(createdBooking, cancellationToken);
                }

                var result = new CreateBookingResult
                {
                    Id = createdBooking.Id,
                    BookingNumber = createdBooking.BookingNumber,
                    Status = createdBooking.Status,
                    TotalAmount = createdBooking.TotalAmount.Amount,
                    Currency = createdBooking.TotalAmount.Currency,
                    TotalTickets = createdBooking.TotalTickets,
                    RequiresApproval = createdBooking.RequiresApproval,
                    BookingDate = createdBooking.BookingDate,
                    CheckInCode = createdBooking.CheckInCode ?? string.Empty
                };

                _logger.LogInformation("Booking created successfully: {BookingId} for event: {EventId} by user: {UserId}",
                    createdBooking.Id, request.EventId, request.UserId);

                return Result<CreateBookingResult>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating booking for event: {EventId} by user: {UserId}",
                    request.EventId, request.UserId);
                return Result<CreateBookingResult>.Failure("An error occurred while creating the booking");
            }
        }
    }
}
