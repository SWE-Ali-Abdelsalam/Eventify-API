using Eventify.Application.Common.Interfaces;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Entities.Events;
using Eventify.Domain.Entities.Identity;
using Eventify.Domain.Entities.Payments;
using Eventify.Domain.Entities.Sponsors;
using Eventify.Domain.Entities.Supporting;
using Eventify.Domain.Entities.Tickets;
using Eventify.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using System.Reflection;

namespace Eventify.Persistence.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid,
    Microsoft.AspNetCore.Identity.IdentityUserClaim<Guid>, ApplicationUserRole,
    Microsoft.AspNetCore.Identity.IdentityUserLogin<Guid>, ApplicationRoleClaim,
    Microsoft.AspNetCore.Identity.IdentityUserToken<Guid>>, IApplicationDbContext
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IDateTime _dateTime;
        private IDbContextTransaction? _currentTransaction;

        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options,
            ICurrentUserService currentUserService,
            IDateTime dateTime) : base(options)
        {
            _currentUserService = currentUserService;
            _dateTime = dateTime;
        }

        // Domain DbSets
        public new DbSet<User> Users => Set<User>();
        public new DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserPreference> UserPreferences => Set<UserPreference>();
        public DbSet<Event> Events => Set<Event>();
        public DbSet<EventCategory> EventCategories => Set<EventCategory>();
        public DbSet<EventSession> EventSessions => Set<EventSession>();
        public DbSet<Venue> Venues => Set<Venue>();
        public DbSet<TicketType> TicketTypes => Set<TicketType>();
        public DbSet<Booking> Bookings => Set<Booking>();
        public DbSet<BookingTicket> BookingTickets => Set<BookingTicket>();
        public DbSet<BookingModification> BookingModifications => Set<BookingModification>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<PaymentRefund> PaymentRefunds => Set<PaymentRefund>();
        public DbSet<SponsorProfile> SponsorProfiles => Set<SponsorProfile>();
        public DbSet<EventSponsor> EventSponsors => Set<EventSponsor>();
        public DbSet<EventReview> EventReviews => Set<EventReview>();
        public DbSet<EventDocument> EventDocuments => Set<EventDocument>();
        public DbSet<EventImage> EventImages => Set<EventImage>();

        // Identity DbSets
        public DbSet<UserLoginHistory> UserLoginHistory => Set<UserLoginHistory>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Apply Identity configurations first
            base.OnModelCreating(builder);

            // Apply all custom configurations
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            // Configure enums as strings globally
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType.IsEnum)
                    {
                        property.SetProviderClrType(typeof(string));
                    }
                }
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = _currentUserService.UserId;
                        entry.Entity.CreatedAt = _dateTime.Now;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = _currentUserService.UserId;
                        entry.Entity.LastModifiedAt = _dateTime.Now;
                        break;
                }
            }

            foreach (var entry in ChangeTracker.Entries<ISoftDeletable>())
            {
                switch (entry.State)
                {
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.Entity.IsDeleted = true;
                        entry.Entity.DeletedAt = _dateTime.Now;
                        entry.Entity.DeletedBy = _currentUserService.UserId;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_currentTransaction != null)
            {
                return;
            }

            _currentTransaction = await Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await SaveChangesAsync(cancellationToken);
                await _currentTransaction?.CommitAsync(cancellationToken)!;
            }
            catch
            {
                await RollbackTransactionAsync(cancellationToken);
                throw;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await _currentTransaction?.RollbackAsync(cancellationToken)!;
            }
            finally
            {
                _currentTransaction?.Dispose();
                _currentTransaction = null;
            }
        }

        public override void Dispose()
        {
            _currentTransaction?.Dispose();
            base.Dispose();
        }
    }
}
