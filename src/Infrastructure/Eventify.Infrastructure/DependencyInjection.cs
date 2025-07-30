using Eventify.Application.Common.Interfaces;
using Eventify.Application.Common.Models;
using Eventify.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SendGrid.Extensions.DependencyInjection;
using Stripe;
using MyInvoiceService = Eventify.Infrastructure.Services.InvoiceService;

namespace Eventify.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Core services
            services.AddScoped<IDateTime, DateTimeService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            // Authentication services
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAuthenticationService, AuthenticationService>();

            // Payment services
            services.AddScoped<IPaymentService, StripePaymentService>();
            services.AddScoped<IInvoiceService, MyInvoiceService>();

            // Email service
            services.AddSendGrid(options =>
            {
                options.ApiKey = configuration["SendGrid:ApiKey"] ?? throw new InvalidOperationException("SendGrid API key is required");
            });
            services.AddScoped<IEmailService, EmailService>();

            // JWT settings
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

            // Stripe configuration
            StripeConfiguration.ApiKey = configuration["Stripe:SecretKey"] ?? throw new InvalidOperationException("Stripe secret key is required");

            // PDF generation (QuestPDF)
            QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

            return services;
        }
    }
}
