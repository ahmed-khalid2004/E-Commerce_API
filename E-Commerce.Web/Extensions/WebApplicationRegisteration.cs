using DomainLayer.Contracts;
using E_Commerce.Web.CustomMiddleWares;
using Service;
using Services;
using ServicesAbstraction;
using System.Runtime.CompilerServices;

namespace E_Commerce.Web.Extensions
{
    public static class WebApplicationRegisteration
    {
        public static async Task SeedDataBaseASync(this WebApplication app)
        {

            using var scope = app.Services.CreateScope();

            var ObjectOfdataSeeding = scope.ServiceProvider.GetRequiredService<IDataSeeding>();

            await ObjectOfdataSeeding.DataSeedAsync();
            await ObjectOfdataSeeding.IdentityDataSeedAsync();
        }

        public static IApplicationBuilder UseCustomExceptionMiddleWare(this IApplicationBuilder app)
        {
            app.UseMiddleware<CustomExceptionHandlerMiddleWare>();
            return app;
        }

        public static IApplicationBuilder UseSwaggerMiddleWares(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
    }
}
