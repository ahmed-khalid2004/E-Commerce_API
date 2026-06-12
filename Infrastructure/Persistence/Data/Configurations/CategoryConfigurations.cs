using DomainLayer.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    internal class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Table
            builder.ToTable("Categories");

            // PK — inherited from BaseEntity<int>, EF resolves automatically
            builder.HasKey(c => c.Id);

            // Name: required, bounded, unique index
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnType("nvarchar(100)");

            builder.HasIndex(c => c.Name)
                   .IsUnique()
                   .HasDatabaseName("IX_Categories_Name");

            // Description: optional
            builder.Property(c => c.Description)
                   .HasMaxLength(300)
                   .HasColumnType("nvarchar(300)");

            // Navigation configured on ProductCategoryConfigurations — 
            // no relationship configured here to keep concerns separated
        }
    }
}