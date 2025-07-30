using Eventify.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class UserLoginHistoryConfiguration : IEntityTypeConfiguration<UserLoginHistory>
    {
        public void Configure(EntityTypeBuilder<UserLoginHistory> builder)
        {
            builder.ToTable("UserLoginHistory");

            builder.HasKey(lh => lh.Id);

            builder.Property(lh => lh.Id)
                .ValueGeneratedNever();

            builder.Property(lh => lh.IpAddress)
                .HasMaxLength(45); // Supports IPv6

            builder.Property(lh => lh.UserAgent)
                .HasMaxLength(500);

            builder.Property(lh => lh.FailureReason)
                .HasMaxLength(500);

            builder.Property(lh => lh.Location)
                .HasMaxLength(255);

            builder.Property(lh => lh.Device)
                .HasMaxLength(255);

            // Relationships
            builder.HasOne(lh => lh.User)
                .WithMany(u => u.LoginHistory)
                .HasForeignKey(lh => lh.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(lh => lh.UserId);
            builder.HasIndex(lh => lh.LoginTime);
            builder.HasIndex(lh => lh.IsSuccessful);
            builder.HasIndex(lh => lh.IpAddress);
        }
    }
}
