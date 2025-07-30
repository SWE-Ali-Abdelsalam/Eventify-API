using Eventify.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class VenueConfiguration : IEntityTypeConfiguration<Venue>
    {
        public void Configure(EntityTypeBuilder<Venue> builder)
        {
            builder.ToTable("Venues");

            builder.HasKey(v => v.Id);

            builder.Property(v => v.Id)
                .ValueGeneratedNever();

            builder.Property(v => v.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(v => v.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(v => v.Capacity)
                .IsRequired();

            builder.Property(v => v.ImageUrl)
                .HasMaxLength(500);

            builder.Property(v => v.WebsiteUrl)
                .HasMaxLength(500);

            builder.Property(v => v.ContactEmail)
                .HasMaxLength(255);

            builder.Property(v => v.ContactPhone)
                .HasMaxLength(20);

            builder.Property(v => v.Amenities)
                .HasMaxLength(2000);

            // ✅ Value object mapping for Address (Owned Type)
            builder.OwnsOne(v => v.Address, address =>
            {
                address.Property(a => a.Street)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("Address_Street");

                address.Property(a => a.City)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Address_City");

                address.Property(a => a.State)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Address_State");

                address.Property(a => a.Country)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Address_Country");

                address.Property(a => a.PostalCode)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("Address_PostalCode");

                address.Property(a => a.Latitude)
                    .HasPrecision(10, 8)
                    .HasColumnName("Address_Latitude");

                address.Property(a => a.Longitude)
                    .HasPrecision(11, 8)
                    .HasColumnName("Address_Longitude");

                // ✅ Add the index on City + State inside the ownsOne block
                address.HasIndex(a => new { a.City, a.State });
            });

            // Relationships
            builder.HasMany(v => v.Events)
                .WithOne(e => e.Venue)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(v => v.Name);
            builder.HasIndex(v => v.IsActive);
        }
    }
}
