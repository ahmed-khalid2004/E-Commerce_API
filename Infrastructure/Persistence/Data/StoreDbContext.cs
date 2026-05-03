using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DomainLayer.Models.ProductModule;

namespace Persistence.Data
{
    public class StoreDbContext : DbContext

    {
     public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {
        }
     public DbSet<Product> Products { get; set; }
     public DbSet<ProductBrand> ProductBrands { get; set; }
     public DbSet<ProductType> ProductTypes { get; set; }
     protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly( typeof(AssemblyReference).Assembly);
        }
    }
}