namespace Eventify.Shared.Constants
{
    public static class CacheKeys
    {
        public const string UserPrefix = "user:";
        public const string EventPrefix = "event:";
        public const string BookingPrefix = "booking:";

        public static string UserById(Guid userId) => $"{UserPrefix}{userId}";
        public static string EventById(Guid eventId) => $"{EventPrefix}{eventId}";
        public static string BookingById(Guid bookingId) => $"{BookingPrefix}{bookingId}";
    }
}
