using DomainLayer.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    internal class SubCategoryConfigurations : IEntityTypeConfiguration<SubCategory>
    {
        public void Configure(EntityTypeBuilder<SubCategory> builder)
        {
            builder.ToTable("SubCategories");
            builder.HasKey(s => s.Id);

            builder.Property(s => s.Name)
                   .IsRequired()
                   .HasMaxLength(100)
                   .HasColumnType("nvarchar(100)");

            // A SubCategory belongs to exactly one Category — 1-to-many.
            // Restrict delete: you can't delete a Category that still has SubCategories,
            // forcing an explicit cleanup decision instead of silent cascading data loss.
            builder.HasOne(s => s.Category)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(s => s.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(s => s.CategoryId)
                   .HasDatabaseName("IX_SubCategories_CategoryId");

            // Optional: prevent two SubCategories with the same name under the same Category
            builder.HasIndex(s => new { s.CategoryId, s.Name })
                   .IsUnique()
                   .HasDatabaseName("IX_SubCategories_CategoryId_Name");
        }
    }
}