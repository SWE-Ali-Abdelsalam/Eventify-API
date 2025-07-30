namespace Eventify.Application.Features.Bookings.DTOs
{
    public class AttendeeInfo
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? SpecialRequests { get; set; }
    }
}
