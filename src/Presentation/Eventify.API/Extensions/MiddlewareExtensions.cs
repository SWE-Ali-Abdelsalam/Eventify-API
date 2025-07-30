using Eventify.API.Middleware;

namespace Eventify.API.Extensions
{
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseResourceOwnershipValidation(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ResourceOwnershipMiddleware>();
        }
    }
}
