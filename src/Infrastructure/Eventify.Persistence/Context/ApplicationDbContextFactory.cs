using Eventify.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Eventify.Persistence.Context
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Eventify.API");

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new ApplicationDbContext(
                builder.Options,
                new DummyCurrentUserService(),
                new DummyDateTime());
        }

        private class DummyCurrentUserService : ICurrentUserService
        {
            public string UserId => "00000000-0000-0000-0000-000000000000";
            public string UserName => "MigrationRunner";
            public bool IsAuthenticated => false;

            public bool IsInRole(string role) => false;

            public IEnumerable<string> GetRoles() => new List<string>();
        }

        private class DummyDateTime : IDateTime
        {
            public DateTime Now => DateTime.UtcNow;
            public DateTime UtcNow => DateTime.UtcNow;
        }
    }
}
