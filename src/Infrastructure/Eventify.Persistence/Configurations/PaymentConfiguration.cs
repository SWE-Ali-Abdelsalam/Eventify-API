using Eventify.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Id)
                .ValueGeneratedNever();

            builder.Property(p => p.PaymentNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(p => p.PaymentNumber)
                .IsUnique();

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.Method)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(p => p.Currency)
                .HasMaxLength(3);

            builder.Property(p => p.ExternalTransactionId)
                .HasMaxLength(255);

            builder.Property(p => p.ExternalPaymentId)
                .HasMaxLength(255);

            builder.Property(p => p.FailureReason)
                .HasMaxLength(1000);

            builder.Property(p => p.ProcessorResponse)
                .HasMaxLength(4000); // Increased for larger Stripe responses

            builder.Property(p => p.BillingDetails)
                .HasMaxLength(2000);

            builder.Property(p => p.PaymentMetadata)
                .HasMaxLength(2000);

            // Value object mapping for Money
            builder.OwnsOne(p => p.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("Amount");

                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("AmountCurrency");
            });

            builder.OwnsOne(p => p.RefundedAmount, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("RefundedAmount");

                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("RefundedCurrency");
            });

            // Audit properties
            builder.Property(p => p.CreatedBy)
                .HasMaxLength(255);

            builder.Property(p => p.LastModifiedBy)
                .HasMaxLength(255);

            // Relationships
            builder.HasOne(p => p.Booking)
                .WithMany(b => b.Payments)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Refunds)
                .WithOne(r => r.Payment)
                .HasForeignKey(r => r.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(p => p.Status);
            builder.HasIndex(p => p.BookingId);
            builder.HasIndex(p => p.ExternalTransactionId);
            builder.HasIndex(p => p.ExternalPaymentId);
            builder.HasIndex(p => p.Method);
            builder.HasIndex(p => p.CreatedAt);
        }
    }
}
