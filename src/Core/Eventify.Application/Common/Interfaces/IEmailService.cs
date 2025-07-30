namespace Eventify.Application.Common.Interfaces
{
    public interface IEmailService
    {
        Task<Result> SendEmailAsync(string to, string subject, string body, bool isHtml = true);
        Task<Result> SendEmailVerificationAsync(string email, string firstName, string verificationLink);
        Task<Result> SendPasswordResetAsync(string email, string firstName, string resetLink);
        Task<Result> SendWelcomeEmailAsync(string email, string firstName);
        Task<Result> SendLoginNotificationAsync(string email, string firstName, string ipAddress, DateTime loginTime);
    }
}
