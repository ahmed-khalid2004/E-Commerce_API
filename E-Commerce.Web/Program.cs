using E_Commerce.Web.Extensions;
using Persistence;
using Service;

namespace E_Commerce.Web
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region Services

            builder.Services.AddControllers();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddWebApplicationServices();

            // CORS — strict in production, open in development
            builder.Services.AddCors(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    // Development: allow any origin for local frontend testing
                    options.AddPolicy("CorsPolicy", policy =>
                        policy.AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowAnyOrigin());
                }
                else
                {
                    // Production: only allow configured origins
                    var allowedOrigins = builder.Configuration
                        .GetSection("AllowedOrigins")
                        .Get<string[]>() ?? [];

                    options.AddPolicy("CorsPolicy", policy =>
                        policy.AllowAnyHeader()
                              .AllowAnyMethod()
                              .WithOrigins(allowedOrigins));
                }
            });

            // JWT + Authorization — registered once here only
            builder.Services.AddJWTService(builder.Configuration);

            // Swagger — registered always, but only used in Development middleware
            builder.Services.AddAuthorizationHeader();

            #endregion

            var app = builder.Build();

            await app.SeedDataBaseASync();

            #region Middleware pipeline

            app.UseCustomExceptionMiddleWare();

            // Swagger — development only
            if (app.Environment.IsDevelopment())
                app.UseSwaggerMiddleWares();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            #endregion

            app.Run();
        }
    }
}