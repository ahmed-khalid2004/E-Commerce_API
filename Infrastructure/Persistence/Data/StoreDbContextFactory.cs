using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Persistence.Data
{
    public class StoreDbContextFactory : IDesignTimeDbContextFactory<StoreDbContext>
    {
        public StoreDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=SQL9001.site4now.net;Database=db_acab93_ecommerce;User Id=db_acab93_ecommerce_admin;Password=Ahmed@0111;Encrypt=True;TrustServerCertificate=True");
            return new StoreDbContext(optionsBuilder.Options);
        }
    }
}