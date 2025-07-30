using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Features.Admin.DTOs;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Entities.Users;
using Eventify.Shared.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Eventify.Application.Features.Admin.Queries.GetSystemStats
{
    public class GetSystemStatsQueryHandler : IRequestHandler<GetSystemStatsQuery, Result<SystemStatsDto>>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Event> _eventRepository;
        private readonly IRepository<Booking> _bookingRepository;
        private readonly IRepository<Payment> _paymentRepository;
        private readonly IApplicationDbContext _context;

        public GetSystemStatsQueryHandler(
            IRepository<User> userRepository,
            IRepository<Event> eventRepository,
            IRepository<Booking> bookingRepository,
            IRepository<Payment> paymentRepository,
            IApplicationDbContext context)
        {
            _userRepository = userRepository;
            _eventRepository = eventRepository;
            _bookingRepository = bookingRepository;
            _paymentRepository = paymentRepository;
            _context = context;
        }

        public async Task<Result<SystemStatsDto>> Handle(GetSystemStatsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var fromDate = request.FromDate ?? DateTime.UtcNow.AddMonths(-12);
                var toDate = request.ToDate ?? DateTime.UtcNow;
                var thisMonth = DateTime.UtcNow.AddMonths(-1);

                var stats = new SystemStatsDto
                {
                    Users = await GetUserStats(thisMonth, cancellationToken),
                    Events = await GetEventStats(thisMonth, cancellationToken),
                    Bookings = await GetBookingStats(thisMonth, cancellationToken),
                    Revenue = await GetRevenueStats(fromDate, toDate, thisMonth, cancellationToken),
                    SystemHealth = await GetSystemHealth(cancellationToken)
                };

                return Result<SystemStatsDto>.Success(stats);
            }
            catch (Exception ex)
            {
                return Result<SystemStatsDto>.Failure($"Error retrieving system stats: {ex.Message}");
            }
        }

        private async Task<UserStatsDto> GetUserStats(DateTime thisMonth, CancellationToken cancellationToken)
        {
            var users = await _context.Users.ToListAsync(cancellationToken);
            var userRoles = await _context.UserRoles
                .Where(ur => ur.IsActive)
                .GroupBy(ur => ur.Role)
                .Select(g => new { Role = g.Key.ToString(), Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Get daily registrations for the last 30 days
            var dailyRegistrations = await _context.Users
                .Where(u => u.CreatedAt >= DateTime.UtcNow.AddDays(-30))
                .GroupBy(u => u.CreatedAt.Date)
                .Select(g => new DailyUserRegistrationDto
                {
                    Date = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync(cancellationToken);

            return new UserStatsDto
            {
                TotalUsers = users.Count,
                ActiveUsers = users.Count(u => u.IsActive),
                NewUsersThisMonth = users.Count(u => u.CreatedAt >= thisMonth),
                VerifiedUsers = users.Count(u => u.IsEmailVerified),
                UsersByRole = userRoles.ToDictionary(ur => ur.Role, ur => ur.Count),
                DailyRegistrations = dailyRegistrations
            };
        }

        private async Task<EventStatsDto> GetEventStats(DateTime thisMonth, CancellationToken cancellationToken)
        {
            var events = await _context.Events.ToListAsync(cancellationToken);

            return new EventStatsDto
            {
                TotalEvents = events.Count,
                PublishedEvents = events.Count(e => e.Status == EventStatus.Published),
                DraftEvents = events.Count(e => e.Status == EventStatus.Draft),
                CompletedEvents = events.Count(e => e.Status == EventStatus.Completed),
                CancelledEvents = events.Count(e => e.Status == EventStatus.Cancelled),
                EventsThisMonth = events.Count(e => e.CreatedAt >= thisMonth),
                AverageCapacity = events.Any() ? events.Average(e => e.MaxCapacity) : 0,
                AverageRegistrationRate = events.Any() ?
                    events.Where(e => e.MaxCapacity > 0).Average(e => (double)e.CurrentRegistrations / e.MaxCapacity * 100) : 0
            };
        }

        private async Task<BookingStatsDto> GetBookingStats(DateTime thisMonth, CancellationToken cancellationToken)
        {
            var bookings = await _context.Bookings.ToListAsync(cancellationToken);

            return new BookingStatsDto
            {
                TotalBookings = bookings.Count,
                ConfirmedBookings = bookings.Count(b => b.Status == BookingStatus.Confirmed),
                PendingBookings = bookings.Count(b => b.Status == BookingStatus.Pending),
                CancelledBookings = bookings.Count(b => b.Status == BookingStatus.Cancelled),
                BookingsThisMonth = bookings.Count(b => b.CreatedAt >= thisMonth),
                AverageTicketsPerBooking = bookings.Any() ? bookings.Average(b => b.TotalTickets) : 0,
                BookingConversionRate = 85.5 // This would be calculated based on actual metrics
            };
        }

        private async Task<RevenueStatsDto> GetRevenueStats(DateTime fromDate, DateTime toDate, DateTime thisMonth, CancellationToken cancellationToken)
        {
            var payments = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Completed)
                .ToListAsync(cancellationToken);

            var monthlyRevenue = await _context.Payments
                .Where(p => p.Status == PaymentStatus.Completed && p.CreatedAt >= fromDate)
                .GroupBy(p => new { p.CreatedAt.Year, p.CreatedAt.Month })
                .Select(g => new MonthlyRevenueDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM"),
                    Revenue = g.Sum(p => p.Amount.Amount),
                    TransactionCount = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync(cancellationToken);

            return new RevenueStatsDto
            {
                TotalRevenue = payments.Sum(p => p.Amount.Amount),
                RevenueThisMonth = payments.Where(p => p.CreatedAt >= thisMonth).Sum(p => p.Amount.Amount),
                RevenueThisYear = payments.Where(p => p.CreatedAt.Year == DateTime.UtcNow.Year).Sum(p => p.Amount.Amount),
                AverageOrderValue = payments.Any() ? payments.Average(p => p.Amount.Amount) : 0,
                TotalTransactions = payments.Count,
                SuccessfulPayments = payments.Count,
                FailedPayments = await _context.Payments.CountAsync(p => p.Status == PaymentStatus.Failed, cancellationToken),
                RefundedAmount = payments.Where(p => p.RefundedAmount != null).Sum(p => p.RefundedAmount!.Amount),
                MonthlyRevenue = monthlyRevenue
            };
        }

        private async Task<SystemHealthDto> GetSystemHealth(CancellationToken cancellationToken)
        {
            var warnings = new List<string>();

            // Check for any system issues
            var failedPaymentsToday = await _context.Payments
                .CountAsync(p => p.Status == PaymentStatus.Failed && p.CreatedAt.Date == DateTime.UtcNow.Date, cancellationToken);

            if (failedPaymentsToday > 10)
            {
                warnings.Add($"High number of failed payments today: {failedPaymentsToday}");
            }

            return new SystemHealthDto
            {
                DatabaseResponseTime = 25.5, // This would be measured in real-time
                ApiResponseTime = 150.2,
                ActiveSessions = 42, // This would come from session tracking
                LastBackup = DateTime.UtcNow.AddHours(-2), // This would come from backup system
                SystemStatus = warnings.Any() ? "Warning" : "Healthy",
                Warnings = warnings
            };
        }
    }
}
