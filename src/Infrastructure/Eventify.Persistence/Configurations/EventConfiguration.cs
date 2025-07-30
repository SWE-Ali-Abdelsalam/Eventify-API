using Eventify.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.ToTable("Events");

            builder.HasKey(e => e.Id);

            builder.Property(e => e.Id)
                .ValueGeneratedNever();

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(5000);

            builder.Property(e => e.ShortDescription)
                .HasMaxLength(500);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>();

            // Value object mapping for DateRange
            builder.OwnsOne(e => e.EventDates, dates =>
            {
                dates.Property(d => d.StartDate)
                    .IsRequired()
                    .HasColumnName("StartDate");

                dates.Property(d => d.EndDate)
                    .IsRequired()
                    .HasColumnName("EndDate");
            });

            builder.Property(e => e.MaxCapacity)
                .IsRequired();

            builder.Property(e => e.CurrentRegistrations)
                .IsRequired();

            builder.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            builder.Property(e => e.WebsiteUrl)
                .HasMaxLength(500);

            builder.Property(e => e.Tags)
                .HasMaxLength(1000);

            builder.Property(e => e.CustomFields)
                .HasMaxLength(4000);

            builder.Property(e => e.VirtualMeetingUrl)
                .HasMaxLength(500);

            builder.Property(e => e.VirtualMeetingPassword)
                .HasMaxLength(100);

            // Audit properties
            builder.Property(e => e.CreatedBy)
                .HasMaxLength(255);

            builder.Property(e => e.LastModifiedBy)
                .HasMaxLength(255);

            builder.Property(e => e.DeletedBy)
                .HasMaxLength(255);

            // Relationships
            builder.HasOne(e => e.Organizer)
                .WithMany(u => u.OrganizedEvents)
                .HasForeignKey(e => e.OrganizerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Category)
                .WithMany(c => c.Events)
                .HasForeignKey(e => e.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(e => e.Venue)
                .WithMany(v => v.Events)
                .HasForeignKey(e => e.VenueId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(e => e.Sessions)
                .WithOne(s => s.Event)
                .HasForeignKey(s => s.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.TicketTypes)
                .WithOne(t => t.Event)
                .HasForeignKey(t => t.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Bookings)
                .WithOne(b => b.Event)
                .HasForeignKey(b => b.EventId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(e => e.EventSponsors)
                .WithOne(es => es.Event)
                .HasForeignKey(es => es.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Reviews)
                .WithOne(r => r.Event)
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Documents)
                .WithOne(d => d.Event)
                .HasForeignKey(d => d.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(e => e.Images)
                .WithOne(i => i.Event)
                .HasForeignKey(i => i.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            // Indexes
            builder.HasIndex(e => e.Status);
            builder.HasIndex(e => e.OrganizerId);
            builder.HasIndex(e => e.CategoryId);
            builder.HasIndex("StartDate", "EndDate");
            builder.HasIndex(e => e.IsPublic);

            // Global query filter for soft delete
            builder.HasQueryFilter(e => !e.IsDeleted);
        }
    }
}
