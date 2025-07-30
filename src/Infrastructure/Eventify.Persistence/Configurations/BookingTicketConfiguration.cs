using Eventify.Domain.Entities.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class BookingTicketConfiguration : IEntityTypeConfiguration<BookingTicket>
    {
        public void Configure(EntityTypeBuilder<BookingTicket> builder)
        {
            builder.ToTable("BookingTickets");

            builder.HasKey(bt => bt.Id);

            builder.Property(bt => bt.Id)
                .ValueGeneratedNever();

            builder.Property(bt => bt.TicketNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(bt => bt.AttendeeFirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(bt => bt.AttendeeLastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(bt => bt.AttendeeEmail)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(bt => bt.AttendeePhone)
                .HasMaxLength(20);

            builder.Property(bt => bt.QRCode)
                .HasMaxLength(500);

            builder.Property(bt => bt.CheckInLocation)
                .HasMaxLength(255);

            builder.Property(bt => bt.SpecialRequests)
                .HasMaxLength(1000);

            builder.Property(bt => bt.TransferredTo)
                .HasMaxLength(255);

            // ✅ Configure Price (Money Value Object)
            builder.OwnsOne(bt => bt.Price, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("Price");

                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("Currency");
            });

            // Relationships
            builder.HasOne(bt => bt.Booking)
                .WithMany(b => b.Tickets)
                .HasForeignKey(bt => bt.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(bt => bt.TicketType)
                .WithMany()
                .HasForeignKey(bt => bt.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
