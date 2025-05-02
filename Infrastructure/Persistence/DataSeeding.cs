using DomainLayer.Contracts;
using DomainLayer.Models;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistence
{
    public class DataSeeding(StoreDbContext _dbContext) : IDataSeeding
    {
        public void DataSeed()
        {
            if (_dbContext.Database.GetPendingMigrations().Any())
            {
                _dbContext.Database.Migrate();
            }

            if (!_dbContext.ProductBrands.Any())
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), @"..\Infrastructure\Persistence\Data\DataSeed\brands.json"); 
                var ProductBrandData = File.ReadAllText(path);
                var ProductBrands = JsonSerializer.Deserialize<List<ProductBrand>>(ProductBrandData);
                if (ProductBrands != null && ProductBrands.Any())
                {
                    _dbContext.ProductBrands.AddRange(ProductBrands); 
                    _dbContext.SaveChanges();
                }
            }

            if (!_dbContext.ProductTypes.Any())
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), @"..\Infrastructure\Persistence\Data\DataSeed\types.json");
                var ProductTypeData = File.ReadAllText(path);
                var ProductTypes = JsonSerializer.Deserialize<List<ProductType>>(ProductTypeData);
                if (ProductTypes != null && ProductTypes.Any())
                {
                    _dbContext.ProductTypes.AddRange(ProductTypes);
                    _dbContext.SaveChanges();
                }
            }

            if (!_dbContext.Products.Any())
            {
                var path = Path.Combine(Directory.GetCurrentDirectory(), @"..\Infrastructure\Persistence\Data\DataSeed\types.json");
                var ProductData = File.ReadAllText(path);
                var Products = JsonSerializer.Deserialize<List<Product>>(ProductData);
                if (Products != null && Products.Any())
                {
                    _dbContext.Products.AddRange(Products);
                }
            }
            _dbContext.SaveChanges();
        }
    }
}
