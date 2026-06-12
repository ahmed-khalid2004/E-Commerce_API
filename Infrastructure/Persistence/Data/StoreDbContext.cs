using DomainLayer.Models.ProductModule;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {
        }

        // Existing — untouched
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> ProductBrands { get; set; }
        public DbSet<ProductType> ProductTypes { get; set; }

        // New — additive only
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Scans assembly for all IEntityTypeConfiguration<T> implementations,
            // including CategoryConfigurations and ProductCategoryConfigurations.
            // No manual registration needed — existing pattern preserved.
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AssemblyReference).Assembly);
        }
    }
}