using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Bookings.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Specifications.BookingSpecifications;
using MediatR;

namespace Eventify.Application.Features.Bookings.Queries.GetUserBookings
{
    public class GetUserBookingsQueryHandler : IRequestHandler<GetUserBookingsQuery, Result<List<UserBookingDto>>>
    {
        private readonly IRepository<Booking> _bookingRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetUserBookingsQueryHandler(
            IRepository<Booking> bookingRepository,
            ICurrentUserService currentUserService)
        {
            _bookingRepository = bookingRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<List<UserBookingDto>>> Handle(GetUserBookingsQuery request, CancellationToken cancellationToken)
        {
            // Check if user has permission to view these bookings
            if (request.UserId.ToString() != _currentUserService.UserId && !_currentUserService.IsInRole("Admin"))
            {
                return Result<List<UserBookingDto>>.Failure("You don't have permission to view these bookings");
            }

            BaseSpecification<Booking> bookingsSpec;

            if (request.Status.HasValue)
            {
                bookingsSpec = new BookingsByUserAndStatusSpecification(request.UserId, request.Status.Value);
            }
            else
            {
                bookingsSpec = new BookingsByUserSpecification(request.UserId);
            }


            bookingsSpec.ApplyPaging((request.PageNumber - 1) * request.PageSize, request.PageSize);

            var bookings = await _bookingRepository.ListBySpecAsync(bookingsSpec, cancellationToken);

            var userBookingDtos = bookings.Select(b => new UserBookingDto
            {
                Id = b.Id,
                BookingNumber = b.BookingNumber,
                Status = b.Status.ToString(),
                BookingDate = b.BookingDate,
                TotalTickets = b.TotalTickets,
                TotalAmount = b.TotalAmount.Amount,
                Currency = b.TotalAmount.Currency,
                EventTitle = b.Event.Title,
                EventStartDate = b.Event.EventDates.StartDate,
                EventImageUrl = b.Event.ImageUrl,
                RequiresApproval = b.RequiresApproval,
                CanBeCancelled = b.CanBeCancelled,
                IsCheckedIn = b.IsCheckedIn
            }).ToList();

            return Result<List<UserBookingDto>>.Success(userBookingDtos);
        }
    }
}
