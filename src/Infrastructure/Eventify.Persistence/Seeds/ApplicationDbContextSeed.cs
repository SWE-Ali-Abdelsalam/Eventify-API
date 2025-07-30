using Eventify.Domain.Entities.Events;
using Eventify.Domain.Entities.Users;
using Eventify.Domain.ValueObjects;
using Eventify.Persistence.Context;
using Eventify.Shared.Enums;
using Microsoft.EntityFrameworkCore;

namespace Eventify.Persistence.Seeds
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedDefaultDataAsync(ApplicationDbContext context)
        {
            // Seed Event Categories
            if (!await context.EventCategories.AnyAsync())
            {
                await SeedEventCategories(context);
            }

            // Seed Admin User
            if (!await context.Users.AnyAsync())
            {
                await SeedAdminUser(context);
            }

            // Seed Sample Venues
            if (!await context.Venues.AnyAsync())
            {
                await SeedSampleVenues(context);
            }

            await context.SaveChangesAsync();
        }

        private static Task SeedEventCategories(ApplicationDbContext context)
        {
            var categories = new List<EventCategory>
            {
                new EventCategory("Conference", "Professional conferences and business events", null, "#007bff"),
                new EventCategory("Workshop", "Educational workshops and training sessions", null, "#28a745"),
                new EventCategory("Networking", "Networking events and meetups", null, "#ffc107"),
                new EventCategory("Seminar", "Seminars and educational talks", null, "#17a2b8"),
                new EventCategory("Trip", "Travel and leisure trips", null, "#fd7e14"),
                new EventCategory("Sports", "Sports events and activities", null, "#e83e8c"),
                new EventCategory("Entertainment", "Entertainment and cultural events", null, "#6f42c1"),
                new EventCategory("Charity", "Charity events and fundraisers", null, "#20c997"),
                new EventCategory("Other", "Other types of events", null, "#6c757d")
            };

            int order = 1;
            foreach (var category in categories)
            {
                category.UpdateSortOrder(order++);
            }

            context.EventCategories.AddRange(categories);

            return Task.CompletedTask;
        }


        private static async Task SeedAdminUser(ApplicationDbContext context)
        {
            var adminUser = new User("System", "Administrator", "admin@eventmanagement.com");
            adminUser.VerifyEmail();
            adminUser.Activate();

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();

            // Add admin role
            var adminRole = new UserRole(adminUser.Id, UserRoleType.Admin);
            context.UserRoles.Add(adminRole);
        }

        private static Task SeedSampleVenues(ApplicationDbContext context)
        {
            var venues = new List<Venue>
        {
            new Venue(
                "Grand Convention Center",
                "A modern convention center with state-of-the-art facilities",
                new Address("123 Convention Way", "Downtown", "CA", "United States", "12345", 37.7749m, -122.4194m),
                1000
            ),
            new Venue(
                "Tech Hub Auditorium",
                "Perfect venue for tech conferences and startup events",
                new Address("456 Innovation Blvd", "Silicon Valley", "CA", "United States", "54321", 37.3382m, -121.8863m),
                500
            ),
            new Venue(
                "Riverside Conference Hall",
                "Elegant venue with beautiful riverside views",
                new Address("789 River Road", "Riverside", "CA", "United States", "67890", 33.9533m, -117.3962m),
                300
            )
        };

            context.Venues.AddRange(venues);

            return Task.CompletedTask;
        }
    }
}
