using DomainLayer.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    internal class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnType("nvarchar(100)");

            builder.HasIndex(c => c.Name)
                   .IsUnique()
                   .HasDatabaseName("IX_Categories_Name");

            builder.Property(c => c.Description)
                   .HasMaxLength(300)
                   .HasColumnType("nvarchar(300)");

            // Category -> SubCategory relationship is configured on
            // SubCategoryConfigurations to keep concerns separated.
        }
    }
}