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

            builder.Services.AddCors(options =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    options.AddPolicy("CorsPolicy", policy =>
                        policy.AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowAnyOrigin());
                }
                else
                {
                    var allowedOrigins = builder.Configuration
                        .GetSection("AllowedOrigins")
                        .Get<string[]>() ?? [];

                    options.AddPolicy("CorsPolicy", policy =>
                        policy.AllowAnyHeader()
                              .AllowAnyMethod()
                              .WithOrigins(allowedOrigins));
                }
            });

            builder.Services.AddJWTService(builder.Configuration);
            builder.Services.AddAuthorizationHeader();
            #endregion

            var app = builder.Build();

            try { await app.SeedDataBaseASync(); }
            catch (Exception ex) { /* log only */ }

            #region Middleware pipeline
            app.UseCustomExceptionMiddleWare();

            // Swagger — always enabled (needed by frontend team on hosting)
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