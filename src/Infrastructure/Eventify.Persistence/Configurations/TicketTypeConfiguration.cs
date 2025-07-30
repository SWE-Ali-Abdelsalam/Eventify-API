using Eventify.Domain.Entities.Tickets;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class TicketTypeConfiguration : IEntityTypeConfiguration<TicketType>
    {
        public void Configure(EntityTypeBuilder<TicketType> builder)
        {
            builder.ToTable("TicketTypes");

            builder.HasKey(tt => tt.Id);

            builder.Property(tt => tt.Id)
                .ValueGeneratedNever();

            builder.Property(tt => tt.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(tt => tt.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(tt => tt.Quantity)
                .IsRequired();

            builder.Property(tt => tt.SoldQuantity)
                .IsRequired();

            builder.Property(tt => tt.Terms)
                .HasMaxLength(2000);

            builder.Property(tt => tt.SortOrder)
                .IsRequired();

            // Value object mapping for Money
            builder.OwnsOne(tt => tt.Price, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("Price");

                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("Currency");
            });

            // Relationships
            builder.HasOne(tt => tt.Event)
                .WithMany(e => e.TicketTypes)
                .HasForeignKey(tt => tt.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(tt => tt.BookingTickets)
                .WithOne(bt => bt.TicketType)
                .HasForeignKey(bt => bt.TicketTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(tt => tt.EventId);
            builder.HasIndex(tt => tt.IsActive);
            builder.HasIndex(tt => tt.SortOrder);
        }
    }
}
