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

            #region Add Service to Container
            builder.Services.AddControllers();
            //builder.Services.AddSwaggerServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);
            builder.Services.AddApplicationServices();
            builder.Services.AddWebApplicationServices();
            builder.Services.AddJWTService(builder.Configuration);
            builder.Services.AddCors(op =>
            {
                op.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyHeader();
                    builder.AllowAnyMethod();
                    builder.AllowAnyOrigin();
                });
            });
            builder.Services.AddAuthorizationHeader();
            #endregion

            var app = builder.Build();
            await app.SeedDataBaseASync();

            #region Configure the HTTP request pipeline
            app.UseCustomExceptionMiddleWare();
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerMiddleWares();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseCors("AllowAll");
            app.MapControllers();

            #endregion


            app.Run();
        }
    }
}
