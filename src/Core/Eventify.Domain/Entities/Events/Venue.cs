using Eventify.Domain.Common;
using Eventify.Domain.ValueObjects;

namespace Eventify.Domain.Entities.Events
{
    public class Venue : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public string Description { get; private set; } = string.Empty;
        public Address Address { get; private set; } = null!;
        public int Capacity { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? WebsiteUrl { get; private set; }
        public string? ContactEmail { get; private set; }
        public string? ContactPhone { get; private set; }
        public string? Amenities { get; private set; } // JSON string
        public bool IsActive { get; private set; } = true;

        // Navigation properties
        public ICollection<Event> Events { get; private set; } = new List<Event>();

        private Venue() { } // EF Core

        public Venue(string name, string description, Address address, int capacity)
        {
            Name = name;
            Description = description;
            Address = address;
            Capacity = capacity;
        }

        public void UpdateDetails(string name, string description, int capacity, string? imageUrl,
                                 string? websiteUrl, string? contactEmail, string? contactPhone)
        {
            Name = name;
            Description = description;
            Capacity = capacity;
            ImageUrl = imageUrl;
            WebsiteUrl = websiteUrl;
            ContactEmail = contactEmail;
            ContactPhone = contactPhone;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAddress(Address address)
        {
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateAmenities(string amenities)
        {
            Amenities = amenities;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
