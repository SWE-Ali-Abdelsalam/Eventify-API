using Eventify.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class EventCategoryConfiguration : IEntityTypeConfiguration<EventCategory>
    {
        public void Configure(EntityTypeBuilder<EventCategory> builder)
        {
            builder.ToTable("EventCategories");

            builder.HasKey(ec => ec.Id);

            builder.Property(ec => ec.Id)
                .ValueGeneratedNever();

            builder.Property(ec => ec.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasIndex(ec => ec.Name)
                .IsUnique();

            builder.Property(ec => ec.Description)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(ec => ec.IconUrl)
                .HasMaxLength(500);

            builder.Property(ec => ec.Color)
                .HasMaxLength(7); // Hex color code

            builder.Property(ec => ec.SortOrder)
                .IsRequired();

            // Relationships
            builder.HasMany(ec => ec.Events)
                .WithOne(e => e.Category)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(ec => ec.IsActive);
            builder.HasIndex(ec => ec.SortOrder);
        }
    }
}
