using DomainLayer.Models.ProductModule;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Data.Configurations
{
    class ProductConfigurations : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.HasOne(P => P.ProductBrand)
            .WithMany()
            .HasForeignKey(P => P.BrandId);

            builder.HasOne(P => P.ProductType)
             .WithMany()
             .HasForeignKey(P => P.TypeId);

            builder.Property(P => P.Price)
             .HasColumnType("decimal(10,2)");
        }
    }
}

