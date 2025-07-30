using Eventify.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Id)
                .ValueGeneratedNever();

            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(u => u.Email)
                .IsUnique();

            builder.Property(u => u.PhoneNumber)
                .HasMaxLength(20);

            builder.Property(u => u.ProfileImageUrl)
                .HasMaxLength(500);

            builder.Property(u => u.Bio)
                .HasMaxLength(1000);

            builder.Property(u => u.Website)
                .HasMaxLength(255);

            builder.Property(u => u.Company)
                .HasMaxLength(255);

            builder.Property(u => u.JobTitle)
                .HasMaxLength(255);

            builder.Property(u => u.TimeZone)
                .HasMaxLength(50);

            builder.Property(u => u.Language)
                .HasMaxLength(10);

            // Value object mapping for Address
            builder.OwnsOne(u => u.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Street");

                address.Property(a => a.City)
                    .HasMaxLength(100)
                    .HasColumnName("Address_City");

                address.Property(a => a.State)
                    .HasMaxLength(100)
                    .HasColumnName("Address_State");

                address.Property(a => a.Country)
                    .HasMaxLength(100)
                    .HasColumnName("Address_Country");

                address.Property(a => a.PostalCode)
                    .HasMaxLength(20)
                    .HasColumnName("Address_PostalCode");

                address.Property(a => a.Latitude)
                    .HasPrecision(10, 8)
                    .HasColumnName("Address_Latitude");

                address.Property(a => a.Longitude)
                    .HasPrecision(11, 8)
                    .HasColumnName("Address_Longitude");
            });

            // Audit properties
            builder.Property(u => u.CreatedBy)
                .HasMaxLength(255);

            builder.Property(u => u.LastModifiedBy)
                .HasMaxLength(255);

            builder.Property(u => u.DeletedBy)
                .HasMaxLength(255);

            // Relationships
            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.OrganizedEvents)
                .WithOne(e => e.Organizer)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Bookings)
                .WithOne(b => b.User)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.SponsorProfiles)
                .WithOne(sp => sp.User)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.EventReviews)
                .WithOne(er => er.User)
                .HasForeignKey(er => er.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.Preferences)
                .WithOne(up => up.User)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Global query filter for soft delete
            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
