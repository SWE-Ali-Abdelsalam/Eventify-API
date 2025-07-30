using Eventify.Shared.Enums;

namespace Eventify.API.Extensions
{
    public static class AuthorizationExtensions
    {
        public static IServiceCollection AddCustomAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Role-based policies
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole(UserRoleType.Admin.ToString()));

                options.AddPolicy("OrganizerOnly", policy =>
                    policy.RequireRole(UserRoleType.Organizer.ToString(), UserRoleType.Admin.ToString()));

                options.AddPolicy("SponsorOnly", policy =>
                    policy.RequireRole(UserRoleType.Sponsor.ToString(), UserRoleType.Admin.ToString()));

                options.AddPolicy("ParticipantOnly", policy =>
                    policy.RequireRole(UserRoleType.Participant.ToString(), UserRoleType.Admin.ToString()));

                // Claims-based policies
                options.AddPolicy("CanManageEvents", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole(UserRoleType.Admin.ToString()) ||
                        context.User.IsInRole(UserRoleType.Organizer.ToString())));

                options.AddPolicy("CanManageSponsors", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole(UserRoleType.Admin.ToString()) ||
                        context.User.IsInRole(UserRoleType.Sponsor.ToString())));

                options.AddPolicy("CanViewReports", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole(UserRoleType.Admin.ToString()) ||
                        context.User.IsInRole(UserRoleType.Organizer.ToString())));

                // Email verification requirement
                options.AddPolicy("EmailVerified", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("email_verified", "true")));

                // Minimum requirements
                options.AddPolicy("ActiveUser", policy =>
                    policy.RequireAssertion(context =>
                        context.User.Identity != null &&
                        context.User.Identity.IsAuthenticated &&
                        !context.User.HasClaim("account_status", "inactive")));
            });

            return services;
        }
    }
}
