using Eventify.Domain.Common;
using Eventify.Domain.Entities.Identity;
using Eventify.Domain.Entities.Users;
using Eventify.Persistence.Context;
using Eventify.Shared.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Eventify.Persistence.Seeds
{
    public static class IdentitySeedData
    {
        public static async Task SeedDefaultRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var userRepository = serviceProvider.GetRequiredService<IRepository<User>>();
            var logger = serviceProvider.GetRequiredService<ILogger<ApplicationDbContext>>();

            try
            {
                // Seed roles
                await SeedRolesAsync(roleManager, logger);

                // Seed admin user
                await SeedAdminUserAsync(userManager, userRepository, logger);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding identity data");
                throw;
            }
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager, ILogger logger)
        {
            var roles = new[]
            {
                new ApplicationRole(UserRoleType.Admin.ToString(), "System administrators with full access", true),
                new ApplicationRole(UserRoleType.Organizer.ToString(), "Event organizers who can create and manage events", true),
                new ApplicationRole(UserRoleType.Participant.ToString(), "Event participants who can register for events", true),
                new ApplicationRole(UserRoleType.Sponsor.ToString(), "Event sponsors who can sponsor events", true)
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role.Name!))
                {
                    var result = await roleManager.CreateAsync(role);
                    if (result.Succeeded)
                    {
                        logger.LogInformation("Created role: {RoleName}", role.Name);
                    }
                    else
                    {
                        logger.LogError("Failed to create role {RoleName}: {Errors}",
                            role.Name, string.Join(", ", result.Errors.Select(e => e.Description)));
                    }
                }
            }
        }

        private static async Task SeedAdminUserAsync(
            UserManager<ApplicationUser> userManager,
            IRepository<User> userRepository,
            ILogger logger)
        {
            const string adminEmail = "admin@eventmanagement.com";
            const string adminPassword = "Admin123!";

            // Check if Domain User already exists
            var users = await userRepository.GetAllAsync();
            var existingDomainUser = users.FirstOrDefault(u => u.Email == adminEmail);

            if (existingDomainUser != null)
            {
                logger.LogInformation("Admin domain user already exists, skipping creation.");
                return;
            }

            // Check if Identity User already exists
            var existingIdentityUser = await userManager.FindByEmailAsync(adminEmail);
            if (existingIdentityUser != null)
            {
                logger.LogInformation("Admin identity user already exists, skipping creation.");
                return;
            }

            // Create domain user
            var domainUser = new User("System", "Administrator", adminEmail);
            domainUser.VerifyEmail();
            domainUser.Activate();

            var createdDomainUser = await userRepository.AddAsync(domainUser);
            logger.LogInformation("Created domain admin user: {UserId}", createdDomainUser.Id);

            // Create identity user
            var identityUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                PhoneNumberConfirmed = false,
                TwoFactorEnabled = false,
                LockoutEnabled = false,
                AccessFailedCount = 0,
                EventManagementUserId = createdDomainUser.Id,
                IsActive = true
            };

            var result = await userManager.CreateAsync(identityUser, adminPassword);
            if (result.Succeeded)
            {
                logger.LogInformation("Created identity admin user: {UserId}", identityUser.Id);

                // Assign to Admin role
                var roleResult = await userManager.AddToRoleAsync(identityUser, UserRoleType.Admin.ToString());
                if (roleResult.Succeeded)
                {
                    logger.LogInformation("Added admin user to Admin role");
                }
                else
                {
                    logger.LogError("Failed to add admin user to role: {Errors}",
                        string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                }

                // Add claims
                var claims = new[]
                {
                    new System.Security.Claims.Claim("email_verified", "true"),
                    new System.Security.Claims.Claim("account_status", "active"),
                    new System.Security.Claims.Claim("user_type", "system_admin")
                };

                var claimsResult = await userManager.AddClaimsAsync(identityUser, claims);
                if (claimsResult.Succeeded)
                {
                    logger.LogInformation("Added claims to admin user");
                }
            }
            else
            {
                // Cleanup domain user if identity creation fails
                await userRepository.DeleteAsync(createdDomainUser);
                logger.LogError("Failed to create identity admin user: {Errors}",
                    string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new InvalidOperationException("Failed to create admin user");
            }
        }
    }
}
