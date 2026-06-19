using DomainLayer.Contracts;
using DomainLayer.Models.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Caching;
using Persistence.Data;
using Persistence.Identity;
using Persistence.Repositories;

namespace Persistence
{
    public static class InfrastructureServicesRegisteration
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── Single connection string for both DbContexts ───────────────────
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Store DbContext — schema: dbo
            services.AddDbContext<StoreDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Identity DbContext — schema: identity — same DB, different schema
            services.AddDbContext<StoreIdentityDbContext>(options =>
                options.UseSqlServer(connectionString));

            // ── Repositories & UnitOfWork ─────────────────────────────────────
            services.AddScoped<IDataSeeding, DataSeeding>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // ── Basket Repository: Redis when enabled, In-Memory fallback otherwise ───
            var cachingEnabled = configuration.GetValue<bool>("Caching:Enabled");
            if (cachingEnabled)
            {
                    services.AddSingleton<IRedisClient>(_ =>
                    {
                        var restUrl = configuration["UpstashSettings:RestUrl"]!;
                        var restToken = configuration["UpstashSettings:RestToken"]!;
                        return new UpstashRedisClient(new HttpClient(), restUrl, restToken);
                    });

                services.AddScoped<ICacheRepository, CacheRepository>();
                services.AddScoped<IBaseketRepository, BasketRepository>();
            }
            else
            {
                services.AddScoped<IBaseketRepository, InMemoryBasketRepository>();
            }


            // services.AddScoped<ICacheRepository, CacheRepository>();
            //services.AddSingleton<IConnectionMultiplexer>(_ =>
            //    ConnectionMultiplexer.Connect(
            //        configuration.GetConnectionString("RedisConnectionString")!));

            // ── ASP.NET Identity ──────────────────────────────────────────────
            services.AddIdentityCore<ApplicationUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<StoreIdentityDbContext>()
                    .AddDefaultTokenProviders();  

            return services;
        }
    }
}