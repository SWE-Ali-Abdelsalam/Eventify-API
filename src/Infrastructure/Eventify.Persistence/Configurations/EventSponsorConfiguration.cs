using Eventify.Domain.Entities.Sponsors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class EventSponsorConfiguration : IEntityTypeConfiguration<EventSponsor>
    {
        public void Configure(EntityTypeBuilder<EventSponsor> builder)
        {
            builder.ToTable("EventSponsors");

            builder.HasKey(es => es.Id);

            builder.Property(es => es.Id)
                   .ValueGeneratedNever();

            builder.Property(es => es.CustomPackageDetails)
                   .HasMaxLength(2000);

            builder.Property(es => es.Benefits)
                   .HasMaxLength(2000);

            builder.Property(es => es.Deliverables)
                   .HasMaxLength(2000);

            builder.Property(es => es.ApprovedBy)
                   .HasMaxLength(255);

            builder.Property(es => es.Notes)
                   .HasMaxLength(1000);

            // ✅ Configure the Money value object
            builder.OwnsOne(es => es.SponsorshipAmount, money =>
            {
                money.Property(m => m.Amount)
                     .HasPrecision(18, 2)
                     .HasColumnName("SponsorshipAmount");

                money.Property(m => m.Currency)
                     .HasMaxLength(3)
                     .HasColumnName("SponsorshipCurrency");
            });

            // Audit properties
            builder.Property(es => es.CreatedBy)
                   .HasMaxLength(255);

            builder.Property(es => es.LastModifiedBy)
                   .HasMaxLength(255);

            // Relationships
            builder.HasOne(es => es.Event)
                   .WithMany(e => e.EventSponsors)
                   .HasForeignKey(es => es.EventId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(es => es.SponsorProfile)
                   .WithMany(sp => sp.EventSponsors)
                   .HasForeignKey(es => es.SponsorProfileId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
