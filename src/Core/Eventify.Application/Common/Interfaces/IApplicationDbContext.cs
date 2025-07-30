using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Entities.Sponsors;
using Eventify.Domain.Entities.Supporting;
using Eventify.Domain.Entities.Tickets;
using Eventify.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Eventify.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<User> Users { get; }
        DbSet<UserRole> UserRoles { get; }
        DbSet<UserPreference> UserPreferences { get; }
        DbSet<Event> Events { get; }
        DbSet<EventCategory> EventCategories { get; }
        DbSet<EventSession> EventSessions { get; }
        DbSet<Venue> Venues { get; }
        DbSet<TicketType> TicketTypes { get; }
        DbSet<Booking> Bookings { get; }
        DbSet<BookingTicket> BookingTickets { get; }
        DbSet<BookingModification> BookingModifications { get; }
        DbSet<Payment> Payments { get; }
        DbSet<PaymentRefund> PaymentRefunds { get; }
        DbSet<SponsorProfile> SponsorProfiles { get; }
        DbSet<EventSponsor> EventSponsors { get; }
        DbSet<EventReview> EventReviews { get; }
        DbSet<EventDocument> EventDocuments { get; }
        DbSet<EventImage> EventImages { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
