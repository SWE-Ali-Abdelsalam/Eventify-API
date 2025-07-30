using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Bookings.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Specifications.BookingSpecifications;
using MediatR;

namespace Eventify.Application.Features.Bookings.Queries.GetBooking
{
    public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, Result<BookingDto>>
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetBookingQueryHandler(
            IRepository<Booking> bookingRepository,
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<BookingDto>> Handle(GetBookingQuery request, CancellationToken cancellationToken)
        {
            var bookingSpec = new BookingWithDetailsSpecification(request.BookingId);
            var booking = await _bookingRepository.GetBySpecAsync(bookingSpec, cancellationToken);

            if (booking == null)
            {
                return Result<BookingDto>.Failure("Booking not found");
            }

            // Check if user has permission to view this booking
            var hasPermission = booking.UserId.ToString() == request.RequestedBy ||
                               _currentUserService.IsInRole("Admin") ||
                               (_currentUserService.IsInRole("Organizer") && booking.Event.OrganizerId.ToString() == _currentUserService.UserId);

            if (!hasPermission)
            {
                return Result<BookingDto>.Failure("You don't have permission to view this booking");
            }

            var bookingDto = new BookingDto
            {
                Id = booking.Id,
                BookingNumber = booking.BookingNumber,
                Status = booking.Status.ToString(),
                BookingDate = booking.BookingDate,
                ConfirmationDate = booking.ConfirmationDate,
                TotalTickets = booking.TotalTickets,
                TotalAmount = booking.TotalAmount.Amount,
                Currency = booking.TotalAmount.Currency,
                DiscountAmount = booking.DiscountAmount?.Amount,
                PromoCode = booking.PromoCode,
                SpecialRequests = booking.SpecialRequests,
                RequiresApproval = booking.RequiresApproval,
                CheckInCode = booking.CheckInCode,
                CheckInTime = booking.CheckInTime,
                IsGroupBooking = booking.IsGroupBooking,
                Event = new EventSummaryDto
                {
                    Id = booking.Event.Id,
                    Title = booking.Event.Title,
                    StartDate = booking.Event.EventDates.StartDate,
                    EndDate = booking.Event.EventDates.EndDate,
                    ImageUrl = booking.Event.ImageUrl,
                    IsVirtual = booking.Event.IsVirtual,
                    VirtualMeetingUrl = booking.Event.VirtualMeetingUrl
                },
                User = new UserSummaryDto
                {
                    Id = booking.User.Id,
                    FullName = booking.User.FullName,
                    Email = booking.User.Email,
                    PhoneNumber = booking.User.PhoneNumber
                },
                Tickets = booking.Tickets.Select(t => new BookingTicketDto
                {
                    Id = t.Id,
                    TicketNumber = t.TicketNumber,
                    AttendeeFullName = t.AttendeeFullName,
                    AttendeeEmail = t.AttendeeEmail,
                    AttendeePhone = t.AttendeePhone,
                    Price = t.Price.Amount,
                    Currency = t.Price.Currency,
                    QRCode = t.QRCode,
                    CheckInTime = t.CheckInTime,
                    TicketTypeName = t.TicketType.Name
                }).ToList(),
                Payments = booking.Payments.Select(p => new PaymentSummaryDto
                {
                    Id = p.Id,
                    PaymentNumber = p.PaymentNumber,
                    Status = p.Status.ToString(),
                    Amount = p.Amount.Amount,
                    Currency = p.Amount.Currency,
                    CompletedAt = p.CompletedAt
                }).ToList()
            };

            return Result<BookingDto>.Success(bookingDto);
        }
    }
}
