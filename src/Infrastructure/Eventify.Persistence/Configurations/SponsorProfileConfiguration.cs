using Eventify.Domain.Entities.Sponsors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class SponsorProfileConfiguration : IEntityTypeConfiguration<SponsorProfile>
    {
        public void Configure(EntityTypeBuilder<SponsorProfile> builder)
        {
            builder.ToTable("SponsorProfiles");

            builder.HasKey(sp => sp.Id);

            builder.Property(sp => sp.Id)
                .ValueGeneratedNever();

            builder.Property(sp => sp.CompanyName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(sp => sp.Industry)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(sp => sp.Description)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(sp => sp.Website)
                .HasMaxLength(500);

            builder.Property(sp => sp.LogoUrl)
                .HasMaxLength(500);

            builder.Property(sp => sp.ContactPersonName)
                .HasMaxLength(255);

            builder.Property(sp => sp.ContactPersonEmail)
                .HasMaxLength(255);

            builder.Property(sp => sp.ContactPersonPhone)
                .HasMaxLength(20);

            builder.Property(sp => sp.SocialMediaLinks)
                .HasMaxLength(2000);

            builder.Property(sp => sp.MarketingMaterials)
                .HasMaxLength(4000);

            // Value object mapping for Address
            builder.OwnsOne(sp => sp.Address, address =>
            {
                address.Property(a => a.Street)
                    .HasMaxLength(255)
                    .HasColumnName("Address_Street");

                address.Property(a => a.City)
                    .HasMaxLength(100)
                    .HasColumnName("Address_City");

                address.Property(a => a.State)
                    .HasMaxLength(100)
                    .HasColumnName("Address_State");

                address.Property(a => a.Country)
                    .HasMaxLength(100)
                    .HasColumnName("Address_Country");

                address.Property(a => a.PostalCode)
                    .HasMaxLength(20)
                    .HasColumnName("Address_PostalCode");

                address.Property(a => a.Latitude)
                    .HasPrecision(10, 8)
                    .HasColumnName("Address_Latitude");

                address.Property(a => a.Longitude)
                    .HasPrecision(11, 8)
                    .HasColumnName("Address_Longitude");
            });

            // Audit properties
            builder.Property(sp => sp.CreatedBy)
                .HasMaxLength(255);

            builder.Property(sp => sp.LastModifiedBy)
                .HasMaxLength(255);

            // Relationships
            builder.HasOne(sp => sp.User)
                .WithMany(u => u.SponsorProfiles)
                .HasForeignKey(sp => sp.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(sp => sp.EventSponsors)
                .WithOne(es => es.SponsorProfile)
                .HasForeignKey(es => es.SponsorProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(sp => sp.UserId);
            builder.HasIndex(sp => sp.CompanyName);
            builder.HasIndex(sp => sp.Industry);
            builder.HasIndex(sp => sp.IsVerified);
            builder.HasIndex(sp => sp.IsActive);
        }
    }
}
