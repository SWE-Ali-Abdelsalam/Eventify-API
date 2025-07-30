using Eventify.Domain.Entities.Bookings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class BookingModificationConfiguration : IEntityTypeConfiguration<BookingModification>
    {
        public void Configure(EntityTypeBuilder<BookingModification> builder)
        {
            builder.ToTable("BookingModifications");

            builder.HasKey(bm => bm.Id);

            builder.Property(bm => bm.Type)
                .IsRequired();

            builder.Property(bm => bm.Description)
                .HasMaxLength(1000)
                .IsRequired();

            builder.Property(bm => bm.OldValue)
                .HasMaxLength(1000);

            builder.Property(bm => bm.NewValue)
                .HasMaxLength(1000);

            builder.Property(bm => bm.RequestedBy)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(bm => bm.ProcessedBy)
                .HasMaxLength(255);

            builder.Property(bm => bm.RequestedAt)
                .IsRequired();

            builder.Property(bm => bm.ProcessedAt);

            // 👇 Configure the Value Object as Owned Type
            builder.OwnsOne(bm => bm.AmountChange, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("AmountChange_Amount");

                money.Property(m => m.Currency)
                    .HasMaxLength(10)
                    .HasColumnName("AmountChange_Currency");
            });

            builder.HasOne(bm => bm.Booking)
                .WithMany(b => b.Modifications)
                .HasForeignKey(bm => bm.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Audit fields (لو عندك BaseEntity فيها مثلا CreatedAt, UpdatedAt)
            builder.Property(bm => bm.CreatedAt);
            builder.Property(bm => bm.UpdatedAt);
        }
    }
}
