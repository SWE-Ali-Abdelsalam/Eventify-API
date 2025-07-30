using Eventify.Domain.Common;
using Eventify.Domain.Entities.Bookings;
using Eventify.Domain.Entities.Events;
using System.Security.Claims;

namespace Eventify.API.Middleware
{
    public class ResourceOwnershipMiddleware
    {
        private readonly RequestDelegate _next;

        public ResourceOwnershipMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IRepository<Event> eventRepository, IRepository<Booking> bookingRepository)
        {
            // Check if this is a resource-specific request that needs ownership validation
            if (ShouldValidateOwnership(context))
            {
                var isAuthorized = await ValidateResourceOwnership(context, eventRepository, bookingRepository);
                if (!isAuthorized)
                {
                    context.Response.StatusCode = 403;
                    await context.Response.WriteAsync("Access denied: You don't have permission to access this resource");
                    return;
                }
            }

            await _next(context);
        }

        private bool ShouldValidateOwnership(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();
            var method = context.Request.Method.ToUpper();

            // Validate ownership for PUT/DELETE operations on events and bookings
            return (path?.Contains("/events/") == true && (method == "PUT" || method == "DELETE")) ||
                   (path?.Contains("/bookings/") == true && (method == "PUT" || method == "DELETE" || method == "POST"));
        }

        private async Task<bool> ValidateResourceOwnership(HttpContext context, IRepository<Event> eventRepository, IRepository<Booking> bookingRepository)
        {
            var user = context.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return false;

            // Admins can access everything
            if (user.IsInRole("Admin"))
                return true;

            var path = context.Request.Path.Value?.ToLower();

            if (path?.Contains("/events/") == true)
            {
                return await ValidateEventOwnership(path, userId, eventRepository);
            }

            if (path?.Contains("/bookings/") == true)
            {
                return await ValidateBookingOwnership(path, userId, bookingRepository);
            }

            return false;
        }

        private async Task<bool> ValidateEventOwnership(string path, string userId, IRepository<Event> eventRepository)
        {
            // Extract event ID from path
            var segments = path.Split('/');
            var eventIdIndex = Array.IndexOf(segments, "events") + 1;

            if (eventIdIndex < segments.Length && Guid.TryParse(segments[eventIdIndex], out var eventId))
            {
                var eventEntity = await eventRepository.GetByIdAsync(eventId);
                return eventEntity?.OrganizerId.ToString() == userId;
            }

            return false;
        }

        private async Task<bool> ValidateBookingOwnership(string path, string userId, IRepository<Booking> bookingRepository)
        {
            // Extract booking ID from path
            var segments = path.Split('/');
            var bookingIdIndex = Array.IndexOf(segments, "bookings") + 1;

            if (bookingIdIndex < segments.Length && Guid.TryParse(segments[bookingIdIndex], out var bookingId))
            {
                var booking = await bookingRepository.GetByIdAsync(bookingId);
                return booking?.UserId.ToString() == userId;
            }

            return false;
        }
    }
}
