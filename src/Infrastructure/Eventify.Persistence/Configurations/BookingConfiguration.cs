using Eventify.Domain.Entities.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            builder.ToTable("Bookings");

            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .ValueGeneratedNever();

            builder.Property(b => b.BookingNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(b => b.BookingNumber)
                .IsUnique();

            builder.Property(b => b.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(b => b.CheckInCode)
                .HasMaxLength(20);

            builder.HasIndex(b => b.CheckInCode)
                .IsUnique();

            builder.Property(b => b.CancellationReason)
                .HasMaxLength(1000);

            builder.Property(b => b.PromoCode)
                .HasMaxLength(50);

            builder.Property(b => b.SpecialRequests)
                .HasMaxLength(2000);

            builder.Property(b => b.AttendeeInformation)
                .HasMaxLength(4000);

            builder.Property(b => b.ApprovedBy)
                .HasMaxLength(255);

            builder.Property(b => b.RejectionReason)
                .HasMaxLength(1000);

            // Value object mapping for Money
            builder.OwnsOne(b => b.TotalAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("TotalAmount");

                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("Currency");
            });

            builder.OwnsOne(b => b.DiscountAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("DiscountAmount");

                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("DiscountCurrency");
            });

            // Audit properties
            builder.Property(b => b.CreatedBy)
                .HasMaxLength(255);

            builder.Property(b => b.LastModifiedBy)
                .HasMaxLength(255);

            // Relationships
            builder.HasOne(b => b.Event)
                .WithMany(e => e.Bookings)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(b => b.User)
                .WithMany(u => u.Bookings)
                .HasForeignKey(b => b.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(b => b.Tickets)
                .WithOne(t => t.Booking)
                .HasForeignKey(t => t.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Payments)
                .WithOne(p => p.Booking)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(b => b.Modifications)
                .WithOne(m => m.Booking)
                .HasForeignKey(m => m.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(b => b.Status);
            builder.HasIndex(b => b.EventId);
            builder.HasIndex(b => b.UserId);
            builder.HasIndex(b => b.BookingDate);
        }
    }
}
