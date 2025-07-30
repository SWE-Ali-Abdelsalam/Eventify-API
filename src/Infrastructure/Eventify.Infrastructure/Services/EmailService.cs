using Eventify.Application.Common;
using Eventify.Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Eventify.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(ISendGridClient sendGridClient, IConfiguration configuration, ILogger<EmailService> logger)
        {
            _sendGridClient = sendGridClient;
            _configuration = configuration;
            _logger = logger;
            _fromEmail = _configuration["SendGrid:FromEmail"] ?? "noreply@eventmanagement.com";
            _fromName = _configuration["SendGrid:FromName"] ?? "Event Management System";
        }

        public async Task<Result> SendEmailAsync(string to, string subject, string body, bool isHtml = true)
        {
            try
            {
                var from = new EmailAddress(_fromEmail, _fromName);
                var toAddress = new EmailAddress(to);

                var msg = MailHelper.CreateSingleEmail(from, toAddress, subject, isHtml ? null : body, isHtml ? body : null);

                var response = await _sendGridClient.SendEmailAsync(msg);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Email sent successfully to {To} with subject: {Subject}", to, subject);
                    return Result.Success();
                }
                else
                {
                    var responseBody = await response.Body.ReadAsStringAsync();
                    _logger.LogError("Failed to send email to {To}. Status: {StatusCode}, Response: {Response}",
                        to, response.StatusCode, responseBody);
                    return Result.Failure($"Failed to send email. Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while sending email to {To}", to);
                return Result.Failure("An error occurred while sending email");
            }
        }

        public async Task<Result> SendEmailVerificationAsync(string email, string firstName, string verificationLink)
        {
            var subject = "Verify Your Email Address";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #007bff;'>Welcome to Event Management System!</h2>
                    <p>Hello {firstName},</p>
                    <p>Thank you for registering with Event Management System. To complete your registration, please verify your email address by clicking the button below:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{verificationLink}' style='background-color: #007bff; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Verify Email Address</a>
                    </div>
                    <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                    <p style='word-break: break-all; color: #007bff;'>{verificationLink}</p>
                    <p>This verification link will expire in 24 hours.</p>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='font-size: 12px; color: #666;'>
                        If you didn't create an account with us, please ignore this email.
                    </p>
                </div>
            </body>
            </html>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<Result> SendPasswordResetAsync(string email, string firstName, string resetLink)
        {
            var subject = "Reset Your Password";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #dc3545;'>Password Reset Request</h2>
                    <p>Hello {firstName},</p>
                    <p>We received a request to reset your password for your Event Management System account. Click the button below to reset your password:</p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='{resetLink}' style='background-color: #dc3545; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Reset Password</a>
                    </div>
                    <p>If the button doesn't work, you can copy and paste this link into your browser:</p>
                    <p style='word-break: break-all; color: #dc3545;'>{resetLink}</p>
                    <p>This password reset link will expire in 1 hour.</p>
                    <p><strong>If you didn't request a password reset, please ignore this email or contact support if you have concerns.</strong></p>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='font-size: 12px; color: #666;'>
                        For security reasons, this link can only be used once and will expire soon.
                    </p>
                </div>
            </body>
            </html>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<Result> SendWelcomeEmailAsync(string email, string firstName)
        {
            var subject = "Welcome to Event Management System!";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #28a745;'>Welcome to Event Management System!</h2>
                    <p>Hello {firstName},</p>
                    <p>Welcome to Event Management System! We're excited to have you as part of our community.</p>
                    <p>With your new account, you can:</p>
                    <ul>
                        <li>Discover and register for amazing events</li>
                        <li>Create and manage your own events</li>
                        <li>Connect with other event attendees</li>
                        <li>Track your event history and preferences</li>
                    </ul>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='https://yourdomain.com/dashboard' style='background-color: #28a745; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Get Started</a>
                    </div>
                    <p>If you have any questions or need assistance, don't hesitate to contact our support team.</p>
                    <p>Best regards,<br>The Event Management Team</p>
                    <hr style='margin: 30px 0; border: none; border-top: 1px solid #eee;'>
                    <p style='font-size: 12px; color: #666;'>
                        You're receiving this email because you created an account with Event Management System.
                    </p>
                </div>
            </body>
            </html>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<Result> SendLoginNotificationAsync(string email, string firstName, string ipAddress, DateTime loginTime)
        {
            var subject = "New Login to Your Account";
            var body = $@"
            <html>
            <body style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
                <div style='max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #ffc107;'>New Login Detected</h2>
                    <p>Hello {firstName},</p>
                    <p>We detected a new login to your Event Management System account:</p>
                    <div style='background-color: #f8f9fa; padding: 15px; border-radius: 4px; margin: 20px 0;'>
                        <p><strong>Time:</strong> {loginTime:yyyy-MM-dd HH:mm:ss} UTC</p>
                        <p><strong>IP Address:</strong> {ipAddress}</p>
                    </div>
                    <p>If this was you, you can safely ignore this email.</p>
                    <p><strong>If this wasn't you, please secure your account immediately by changing your password.</strong></p>
                    <div style='text-align: center; margin: 30px 0;'>
                        <a href='https://yourdomain.com/change-password' style='background-color: #dc3545; color: white; padding: 12px 24px; text-decoration: none; border-radius: 4px; display: inline-block;'>Change Password</a>
                    </div>
                    <p>Best regards,<br>The Event Management Security Team</p>
                </div>
            </body>
            </html>";

            return await SendEmailAsync(email, subject, body);
        }
    }
}
