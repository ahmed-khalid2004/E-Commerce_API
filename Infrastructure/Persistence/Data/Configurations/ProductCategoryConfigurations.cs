using DomainLayer.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    internal class ProductCategoryConfigurations : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            // Table
            builder.ToTable("ProductCategories");

            // Composite PK — enforces uniqueness, prevents duplicate associations
            builder.HasKey(pc => new { pc.ProductId, pc.CategoryId });

            // Product → ProductCategory
            // NO cascade delete: removing a Product does not silently destroy Category links.
            // Change to Cascade if business rule requires it explicitly.
            builder.HasOne(pc => pc.Product)
                   .WithMany(p => p.ProductCategories)
                   .HasForeignKey(pc => pc.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Category → ProductCategory
            // Cascade acceptable here: deleting a Category removes its product associations
            builder.HasOne(pc => pc.Category)
                   .WithMany(c => c.ProductCategories)
                   .HasForeignKey(pc => pc.CategoryId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Selective index for reverse lookup: "all products in a category"
            // (ProductId, CategoryId) composite key already covers forward lookup
            builder.HasIndex(pc => pc.CategoryId)
                   .HasDatabaseName("IX_ProductCategories_CategoryId");
        }
    }
}