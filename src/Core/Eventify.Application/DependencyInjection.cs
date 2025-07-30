using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using AutoMapper;
using Eventify.Application.Extensions;

namespace Eventify.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddApplicationServices(Assembly.GetExecutingAssembly());

            services.AddSingleton<IMapper>(serviceProvider =>
            {
                var loggerFactory = serviceProvider.GetService<Microsoft.Extensions.Logging.ILoggerFactory>();

                var mapperConfig = new MapperConfiguration(cfg =>
                {
                    cfg.AddMaps(Assembly.GetExecutingAssembly());
                }, loggerFactory);

                mapperConfig.AssertConfigurationIsValid();
                return mapperConfig.CreateMapper();
            });

            return services;
        }
    }
}