using Eventify.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
    {
        public void Configure(EntityTypeBuilder<ApplicationUser> builder)
        {
            builder.ToTable("AspNetUsers");

            // Additional properties beyond IdentityUser
            builder.Property(u => u.RefreshToken)
                .HasMaxLength(500);

            builder.Property(u => u.LastLoginIp)
                .HasMaxLength(45); // Supports IPv6

            builder.Property(u => u.PasswordResetToken)
                .HasMaxLength(500);

            builder.Property(u => u.EmailVerificationToken)
                .HasMaxLength(500);

            // Audit properties
            builder.Property(u => u.CreatedBy)
                .HasMaxLength(255);

            builder.Property(u => u.LastModifiedBy)
                .HasMaxLength(255);

            builder.Property(u => u.DeletedBy)
                .HasMaxLength(255);

            // Relationships
            builder.HasOne(u => u.EventManagementUser)
                .WithOne()
                .HasForeignKey<ApplicationUser>(u => u.EventManagementUserId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(u => u.UserRoles)
                .WithOne(ur => ur.User)
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(u => u.LoginHistory)
                .WithOne(lh => lh.User)
                .HasForeignKey(lh => lh.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(u => u.EventManagementUserId);
            builder.HasIndex(u => u.RefreshToken);
            builder.HasIndex(u => u.IsActive);
            builder.HasIndex(u => u.LastLoginAt);

            // Global query filter for soft delete
            builder.HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
