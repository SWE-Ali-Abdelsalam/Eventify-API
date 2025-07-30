using Eventify.Domain.Entities.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class PaymentRefundConfiguration : IEntityTypeConfiguration<PaymentRefund>
    {
        public void Configure(EntityTypeBuilder<PaymentRefund> builder)
        {
            builder.ToTable("PaymentRefunds");

            builder.HasKey(pr => pr.Id);

            builder.Property(pr => pr.Id)
                .ValueGeneratedNever();

            builder.Property(pr => pr.Reason)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(pr => pr.ExternalRefundId)
                .HasMaxLength(255);

            builder.Property(pr => pr.ProcessorResponse)
                .HasMaxLength(2000);

            // Value object mapping for Money
            builder.OwnsOne(pr => pr.Amount, money =>
            {
                money.Property(m => m.Amount)
                    .HasPrecision(18, 2)
                    .HasColumnName("Amount");

                money.Property(m => m.Currency)
                    .HasMaxLength(3)
                    .HasColumnName("Currency");
            });

            // Relationships
            builder.HasOne(pr => pr.Payment)
                .WithMany(p => p.Refunds)
                .HasForeignKey(pr => pr.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(pr => pr.PaymentId);
            builder.HasIndex(pr => pr.RefundedAt);
            builder.HasIndex(pr => pr.ExternalRefundId);
        }
    }
}
