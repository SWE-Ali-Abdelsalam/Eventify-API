using Eventify.Domain.Entities.Events;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eventify.Persistence.Configurations
{
    public class EventSessionConfiguration : IEntityTypeConfiguration<EventSession>
    {
        public void Configure(EntityTypeBuilder<EventSession> builder)
        {
            builder.ToTable("EventSessions");

            builder.HasKey(es => es.Id);

            builder.Property(es => es.Id)
                .ValueGeneratedNever();

            builder.Property(es => es.Title)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(es => es.Description)
                .HasMaxLength(2000);

            builder.Property(es => es.Location)
                .HasMaxLength(500);

            builder.Property(es => es.SpeakerName)
                .HasMaxLength(255);

            builder.Property(es => es.SpeakerBio)
                .HasMaxLength(2000);

            builder.Property(es => es.SpeakerImageUrl)
                .HasMaxLength(1000);

            // ✅ Configure DateRange value object
            builder.OwnsOne(es => es.SessionDates, dr =>
            {
                dr.Property(d => d.StartDate)
                    .HasColumnName("SessionStartDate")
                    .IsRequired();

                dr.Property(d => d.EndDate)
                    .HasColumnName("SessionEndDate")
                    .IsRequired();
            });

            builder.Property(es => es.MaxCapacity)
                .IsRequired();

            builder.Property(es => es.CurrentRegistrations)
                .IsRequired();

            builder.Property(es => es.IsRequired)
                .IsRequired();

            builder.Property(es => es.SortOrder)
                .IsRequired();

            builder.HasOne(es => es.Event)
                .WithMany(e => e.Sessions)
                .HasForeignKey(es => es.EventId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
