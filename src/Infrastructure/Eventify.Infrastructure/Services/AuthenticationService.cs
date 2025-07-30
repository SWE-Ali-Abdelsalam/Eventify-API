using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Eventify.Application.Common.Models;
using Eventify.Shared.Enums;
using Eventify.Domain.Common;
using Eventify.Domain.Entities.Identity;
using Eventify.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Eventify.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IJwtService _jwtService;
        private readonly IEmailService _emailService;
        private readonly IRepository<User> _userRepository;
        private readonly ILogger<AuthenticationService> _logger;
        private readonly IDateTime _dateTime;

        public AuthenticationService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<ApplicationRole> roleManager,
            IJwtService jwtService,
            IEmailService emailService,
            IRepository<User> userRepository,
            ILogger<AuthenticationService> logger,
            IDateTime dateTime)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _jwtService = jwtService;
            _emailService = emailService;
            _userRepository = userRepository;
            _logger = logger;
            _dateTime = dateTime;
        }

        public async Task<Result<AuthenticationResult>> LoginAsync(string email, string password, string? ipAddress = null)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt with non-existent email: {Email}", email);
                    return Result<AuthenticationResult>.Failure("Invalid email or password");
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Login attempt for inactive user: {Email}", email);
                    return Result<AuthenticationResult>.Failure("Account is inactive");
                }

                // Check lockout
                if (await _userManager.IsLockedOutAsync(user))
                {
                    _logger.LogWarning("Login attempt for locked out user: {Email}", email);
                    return Result<AuthenticationResult>.Failure("Account is locked out");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, password, true);

                if (!result.Succeeded)
                {
                    user.RecordFailedLogin();
                    await _userManager.UpdateAsync(user);

                    _logger.LogWarning("Failed login attempt for user: {Email}", email);

                    if (result.IsLockedOut)
                    {
                        return Result<AuthenticationResult>.Failure("Account is locked out due to multiple failed attempts");
                    }

                    return Result<AuthenticationResult>.Failure("Invalid email or password");
                }

                // Successful login
                user.RecordLogin(ipAddress ?? "Unknown");
                await _userManager.UpdateAsync(user);

                // Record login history
                var loginHistory = new UserLoginHistory(user.Id, ipAddress, null, true);
                user.LoginHistory.Add(loginHistory);

                var authResult = await GenerateAuthenticationResultAsync(user);

                _logger.LogInformation("Successful login for user: {Email}", email);

                return Result<AuthenticationResult>.Success(authResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for email: {Email}", email);
                return Result<AuthenticationResult>.Failure("An error occurred during login");
            }
        }

        public async Task<Result<AuthenticationResult>> RegisterAsync(string firstName, string lastName, string email, string password, string? phoneNumber = null)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(email);
                if (existingUser != null)
                {
                    return Result<AuthenticationResult>.Failure("User with this email already exists");
                }

                // Create domain user first
                var domainUser = new User(firstName, lastName, email);
                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    domainUser.UpdateProfile(firstName, lastName, phoneNumber, null, null, null, null, null);
                }

                var createdDomainUser = await _userRepository.AddAsync(domainUser);

                // Create identity user
                var identityUser = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    EventManagementUserId = createdDomainUser.Id,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(identityUser, password);

                if (!result.Succeeded)
                {
                    // Cleanup domain user if identity creation failed
                    await _userRepository.DeleteAsync(createdDomainUser);

                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result<AuthenticationResult>.Failure($"Registration failed: {errors}");
                }

                // Add default participant role
                await _userManager.AddToRoleAsync(identityUser, UserRoleType.Participant.ToString());

                // Generate email verification token
                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(identityUser);
                identityUser.SetEmailVerificationToken(emailToken, _dateTime.UtcNow.AddHours(24));
                await _userManager.UpdateAsync(identityUser);

                // Send verification email
                var verificationLink = $"https://yourdomain.com/verify-email?email={email}&token={Uri.EscapeDataString(emailToken)}";
                await _emailService.SendEmailVerificationAsync(email, firstName, verificationLink);

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(email, firstName);

                var authResult = await GenerateAuthenticationResultAsync(identityUser);

                _logger.LogInformation("User registered successfully: {Email}", email);

                return Result<AuthenticationResult>.Success(authResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration for email: {Email}", email);
                return Result<AuthenticationResult>.Failure("An error occurred during registration");
            }
        }

        public async Task<Result<AuthenticationResult>> RefreshTokenAsync(string token, string refreshToken, string? ipAddress = null)
        {
            try
            {
                var principal = _jwtService.GetPrincipalFromExpiredToken(token);
                if (principal == null)
                {
                    return Result<AuthenticationResult>.Failure("Invalid token");
                }

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Result<AuthenticationResult>.Failure("Invalid token");
                }

                var user = await _userManager.FindByIdAsync(userGuid.ToString());
                if (user == null || !user.IsActive)
                {
                    return Result<AuthenticationResult>.Failure("User not found or inactive");
                }

                if (user.RefreshToken != refreshToken || !user.IsRefreshTokenValid)
                {
                    return Result<AuthenticationResult>.Failure("Invalid refresh token");
                }

                var authResult = await GenerateAuthenticationResultAsync(user);

                _logger.LogInformation("Token refreshed for user: {UserId}", userId);

                return Result<AuthenticationResult>.Success(authResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token refresh");
                return Result<AuthenticationResult>.Failure("An error occurred during token refresh");
            }
        }

        public async Task<Result> RevokeTokenAsync(string token)
        {
            try
            {
                var principal = _jwtService.GetPrincipalFromExpiredToken(token);
                if (principal == null)
                {
                    return Result.Failure("Invalid token");
                }

                var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
                {
                    return Result.Failure("Invalid token");
                }

                var user = await _userManager.FindByIdAsync(userGuid.ToString());
                if (user != null)
                {
                    user.ClearRefreshToken();
                    await _userManager.UpdateAsync(user);
                }

                _logger.LogInformation("Token revoked for user: {UserId}", userId);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during token revocation");
                return Result.Failure("An error occurred during token revocation");
            }
        }

        public async Task<Result> LogoutAsync(string userId)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                {
                    return Result.Failure("Invalid user ID");
                }

                var user = await _userManager.FindByIdAsync(userGuid.ToString());
                if (user != null)
                {
                    user.ClearRefreshToken();
                    await _userManager.UpdateAsync(user);
                }

                _logger.LogInformation("User logged out: {UserId}", userId);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user: {UserId}", userId);
                return Result.Failure("An error occurred during logout");
            }
        }

        public async Task<Result> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Don't reveal that user doesn't exist
                    _logger.LogWarning("Password reset requested for non-existent email: {Email}", email);
                    return Result.Success();
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                user.SetPasswordResetToken(resetToken, _dateTime.UtcNow.AddHours(1));
                await _userManager.UpdateAsync(user);

                // Get domain user for first name
                var domainUser = user.EventManagementUserId.HasValue
                    ? await _userRepository.GetByIdAsync(user.EventManagementUserId.Value)
                    : null;

                var firstName = domainUser?.FirstName ?? "User";
                var resetLink = $"https://yourdomain.com/reset-password?email={email}&token={Uri.EscapeDataString(resetToken)}";

                await _emailService.SendPasswordResetAsync(email, firstName, resetLink);

                _logger.LogInformation("Password reset email sent to: {Email}", email);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during forgot password for email: {Email}", email);
                return Result.Failure("An error occurred while processing your request");
            }
        }

        public async Task<Result> ResetPasswordAsync(string email, string token, string newPassword)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Result.Failure("Invalid email or token");
                }

                if (!user.IsPasswordResetTokenValid || user.PasswordResetToken != token)
                {
                    return Result.Failure("Invalid or expired reset token");
                }

                var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result.Failure($"Password reset failed: {errors}");
                }

                user.ClearPasswordResetToken();
                await _userManager.UpdateAsync(user);

                _logger.LogInformation("Password reset successfully for user: {Email}", email);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password reset for email: {Email}", email);
                return Result.Failure("An error occurred during password reset");
            }
        }

        public async Task<Result> VerifyEmailAsync(string email, string token)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Result.Failure("Invalid email or token");
                }

                if (!user.IsEmailVerificationTokenValid || user.EmailVerificationToken != token)
                {
                    return Result.Failure("Invalid or expired verification token");
                }

                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result.Failure($"Email verification failed: {errors}");
                }

                user.VerifyEmail();
                await _userManager.UpdateAsync(user);

                // Update domain user
                if (user.EventManagementUserId.HasValue)
                {
                    var domainUser = await _userRepository.GetByIdAsync(user.EventManagementUserId.Value);
                    if (domainUser != null)
                    {
                        domainUser.VerifyEmail();
                        await _userRepository.UpdateAsync(domainUser);
                    }
                }

                _logger.LogInformation("Email verified successfully for user: {Email}", email);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during email verification for email: {Email}", email);
                return Result.Failure("An error occurred during email verification");
            }
        }

        public async Task<Result> ResendEmailVerificationAsync(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                if (user.EmailConfirmed)
                {
                    return Result.Failure("Email is already verified");
                }

                var emailToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                user.SetEmailVerificationToken(emailToken, _dateTime.UtcNow.AddHours(24));
                await _userManager.UpdateAsync(user);

                // Get domain user for first name
                var domainUser = user.EventManagementUserId.HasValue
                    ? await _userRepository.GetByIdAsync(user.EventManagementUserId.Value)
                    : null;

                var firstName = domainUser?.FirstName ?? "User";
                var verificationLink = $"https://yourdomain.com/verify-email?email={email}&token={Uri.EscapeDataString(emailToken)}";

                await _emailService.SendEmailVerificationAsync(email, firstName, verificationLink);

                _logger.LogInformation("Email verification resent to: {Email}", email);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during resend email verification for email: {Email}", email);
                return Result.Failure("An error occurred while resending verification email");
            }
        }

        public async Task<Result> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
        {
            try
            {
                if (!Guid.TryParse(userId, out var userGuid))
                {
                    return Result.Failure("Invalid user ID");
                }

                var user = await _userManager.FindByIdAsync(userGuid.ToString());
                if (user == null)
                {
                    return Result.Failure("User not found");
                }

                var result = await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return Result.Failure($"Password change failed: {errors}");
                }

                _logger.LogInformation("Password changed successfully for user: {UserId}", userId);

                return Result.Success();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during password change for user: {UserId}", userId);
                return Result.Failure("An error occurred during password change");
            }
        }

        public async Task<Result<AuthenticationResult>> SocialLoginAsync(string provider, string accessToken, string? ipAddress = null)
        {
            // This would integrate with external providers like Google, Facebook, etc.
            // Implementation depends on the specific provider APIs
            await Task.CompletedTask; // Temporarily
            throw new NotImplementedException("Social login will be implemented in the next phase");
        }

        private async Task<AuthenticationResult> GenerateAuthenticationResultAsync(ApplicationUser user)
        {
            var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new(ClaimTypes.Name, user.UserName ?? string.Empty),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new("jti", Guid.NewGuid().ToString()),
            new("iat", new DateTimeOffset(_dateTime.UtcNow).ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

            // Add role claims
            var roles = await _userManager.GetRolesAsync(user);
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            // Add custom claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            claims.AddRange(userClaims);

            // Generate tokens
            var accessToken = _jwtService.GenerateAccessToken(claims);
            var refreshToken = _jwtService.GenerateRefreshToken();
            var expiresAt = _dateTime.UtcNow.AddMinutes(60); // From JWT settings

            // Update refresh token
            user.UpdateRefreshToken(refreshToken, _dateTime.UtcNow.AddDays(7)); // From JWT settings
            await _userManager.UpdateAsync(user);

            // Get domain user info
            var domainUser = user.EventManagementUserId.HasValue
                ? await _userRepository.GetByIdAsync(user.EventManagementUserId.Value)
                : null;

            var userInfo = new UserInfo
            {
                Id = user.Id,
                FirstName = domainUser?.FirstName ?? string.Empty,
                LastName = domainUser?.LastName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                PhoneNumber = user.PhoneNumber,
                IsEmailVerified = user.EmailConfirmed,
                TwoFactorEnabled = user.TwoFactorEnabled,
                Roles = roles.ToList(),
                Claims = userClaims.ToDictionary(c => c.Type, c => (object)c.Value)
            };

            return new AuthenticationResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                User = userInfo
            };
        }
    }
}
