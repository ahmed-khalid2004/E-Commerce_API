using DomainLayer.Models.IdentityModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Identity
{
    public class StoreIdentityDbContext(DbContextOptions<StoreIdentityDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // All identity tables live in the "identity" schema
            builder.HasDefaultSchema("identity");

            // Explicit table names — clean, no AspNet prefix
            builder.Entity<ApplicationUser>()
                   .ToTable("Users", "identity");

            builder.Entity<IdentityRole>()
                   .ToTable("Roles", "identity");

            builder.Entity<IdentityUserRole<string>>()
                   .ToTable("UserRoles", "identity");

            builder.Entity<IdentityUserClaim<string>>()
                   .ToTable("UserClaims", "identity");

            builder.Entity<IdentityUserLogin<string>>()
                   .ToTable("UserLogins", "identity");

            builder.Entity<IdentityRoleClaim<string>>()
                   .ToTable("RoleClaims", "identity");

            builder.Entity<IdentityUserToken<string>>()
                   .ToTable("UserTokens", "identity");

            builder.Entity<Address>()
                   .ToTable("Addresses", "identity");
        }
    }
}