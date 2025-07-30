using Eventify.Application.Common.Models;

namespace Eventify.Application.Common.Interfaces
{
    public interface IAuthenticationService
    {
        Task<Result<AuthenticationResult>> LoginAsync(string email, string password, string? ipAddress = null);
        Task<Result<AuthenticationResult>> RegisterAsync(string firstName, string lastName, string email, string password, string? phoneNumber = null);
        Task<Result<AuthenticationResult>> RefreshTokenAsync(string token, string refreshToken, string? ipAddress = null);
        Task<Result> RevokeTokenAsync(string token);
        Task<Result> LogoutAsync(string userId);
        Task<Result> ForgotPasswordAsync(string email);
        Task<Result> ResetPasswordAsync(string email, string token, string newPassword);
        Task<Result> VerifyEmailAsync(string email, string token);
        Task<Result> ResendEmailVerificationAsync(string email);
        Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword);
        Task<Result<AuthenticationResult>> SocialLoginAsync(string provider, string accessToken, string? ipAddress = null);
    }
}
