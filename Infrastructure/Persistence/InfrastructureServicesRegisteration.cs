using DomainLayer.Contracts;
using DomainLayer.Models.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Data;
using Persistence.Identity;
using Persistence.Repositories;
using StackExchange.Redis;

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

            // Store DbContext — schema: public
            services.AddDbContext<StoreDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Identity DbContext — schema: identity — same DB, different schema
            services.AddDbContext<StoreIdentityDbContext>(options =>
                options.UseSqlServer(connectionString));

            // ── Repositories & UnitOfWork ─────────────────────────────────────
            services.AddScoped<IDataSeeding, DataSeeding>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IBaseketRepository, BasketRepository>();
            services.AddScoped<ICacheRepository, CacheRepository>();

            // ── Redis ─────────────────────────────────────────────────────────
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(
                    configuration.GetConnectionString("RedisConnectionString")!));

            // ── ASP.NET Identity ──────────────────────────────────────────────
            services.AddIdentityCore<ApplicationUser>()
                    .AddRoles<IdentityRole>()
                    .AddEntityFrameworkStores<StoreIdentityDbContext>();

            return services;
        }
    }
}