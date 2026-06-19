using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence.Identity
{
    public class StoreIdentityDbContextFactory
        : IDesignTimeDbContextFactory<StoreIdentityDbContext>
    {
        public StoreIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StoreIdentityDbContext>();
            optionsBuilder.UseSqlServer(
"Server=localhost;Database=ECommerce;Trusted_Connection=True;TrustServerCertificate=True"); 
            return new StoreIdentityDbContext(optionsBuilder.Options);
        }
    }
}