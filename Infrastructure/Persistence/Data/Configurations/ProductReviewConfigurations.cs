using DomainLayer.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Persistence.Data.Configurations
{
    class ProductReviewConfigurations : IEntityTypeConfiguration<ProductReview>
    {
        public void Configure(EntityTypeBuilder<ProductReview> builder)
        {
            builder.HasOne(r => r.Product)
                   .WithMany(p => p.Reviews)
                   .HasForeignKey(r => r.ProductId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Self-Reference — لازم Restrict عشان SQL Server يرفض Multiple Cascade Paths
            builder.HasOne(r => r.ParentReview)
                   .WithMany(r => r.Replies)
                   .HasForeignKey(r => r.ParentReviewId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Property(r => r.Comment).HasMaxLength(1000);
            builder.Property(r => r.UserDisplayName).HasMaxLength(100);
        }
    }
}