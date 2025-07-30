namespace Eventify.Application.Features.Admin.DTOs
{
    public class SystemHealthDto
    {
        public double DatabaseResponseTime { get; set; }
        public double ApiResponseTime { get; set; }
        public int ActiveSessions { get; set; }
        public DateTime LastBackup { get; set; }
        public string SystemStatus { get; set; } = "Healthy";
        public List<string> Warnings { get; set; } = new();
    }
}
