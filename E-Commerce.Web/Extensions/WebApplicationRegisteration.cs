using DomainLayer.Contracts;
using E_Commerce.Web.CustomMiddleWares;
using Service;
using Services;
using ServicesAbstraction;
using Swashbuckle.AspNetCore.SwaggerUI;
using System.Runtime.CompilerServices;
using System.Text.Json;

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
            app.UseSwaggerUI(O =>
            {
                O.ConfigObject = new ConfigObject()
                {
                    DisplayRequestDuration = true
                };

                O.DocumentTitle = "My E-Commerce API";

                O.JsonSerializerOptions = new JsonSerializerOptions()
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };

                O.DocExpansion(DocExpansion.None);
                O.EnableFilter();
                O.EnablePersistAuthorization();
            });
            return app;
        }
    }
}
