using Microsoft.AspNetCore.Builder;

namespace Lykke.Service.BitcoinGold.API.Middleware
{
    public static class MiddlewareApplicationBuilderExtensions
    {
        public static void UseCustomErrorHandligMiddleware(this IApplicationBuilder app, string componentName)
        {
            app.UseMiddleware<CustomGlobalErrorHandlerMiddleware>(componentName);
        }
    }
}
