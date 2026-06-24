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
                "Server=SQL9001.site4now.net;Database=db_acab93_ecommerce;User Id=db_acab93_ecommerce_admin;Password=Ahmed@0111;Encrypt=True;TrustServerCertificate=True");
            return new StoreIdentityDbContext(optionsBuilder.Options);
        }
    }
}