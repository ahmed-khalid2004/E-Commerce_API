using DomainLayer.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(p => p.ProductBrand)
                   .WithMany(b => b.Products)
                   .HasForeignKey(p => p.BrandId);

            // Was ProductType — now SubCategory
            builder.HasOne(p => p.SubCategory)
                   .WithMany(s => s.Products)
                   .HasForeignKey(p => p.SubCategoryId);

            builder.Property(p => p.Price)
                   .HasColumnType("decimal(10,2)");

            builder.Property(p => p.Discount)
                   .HasColumnType("decimal(5,2)");
        }
    }
}