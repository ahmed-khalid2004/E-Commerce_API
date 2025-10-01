using DomainLayer.Contracts;
using DomainLayer.Models.ProductModule;
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
        public async Task DataSeedAsync()
        {
            try
            {
                var pendingMigrations = await _dbContext.Database.GetPendingMigrationsAsync();
                if (pendingMigrations.Any())
                {
                    await _dbContext.Database.MigrateAsync();
                }

                if (!_dbContext.Set<ProductBrand>().Any())
                {
                    var brandDataPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\Infrastructure\Persistence\Data\DataSeed\brands.json");
                    await using var brandDataStream = File.OpenRead(brandDataPath);
                    var productBrands = await JsonSerializer.DeserializeAsync<List<ProductBrand>>(brandDataStream);

                    if (productBrands != null && productBrands.Any())
                    {
                        await _dbContext.ProductBrands.AddRangeAsync(productBrands);
                    }
                }

                if (!_dbContext.Set<ProductType>().Any())
                {
                    var typeDataPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\Infrastructure\Persistence\Data\DataSeed\types.json");
                    await using var typeDataStream = File.OpenRead(typeDataPath);
                    var productTypes = await JsonSerializer.DeserializeAsync<List<ProductType>>(typeDataStream);

                    if (productTypes != null && productTypes.Any())
                    {
                        await _dbContext.ProductTypes.AddRangeAsync(productTypes);
                    }
                }

                if (!_dbContext.Set<Product>().Any())
                {
                    var productDataPath = Path.Combine(Directory.GetCurrentDirectory(), @"..\Infrastructure\Persistence\Data\DataSeed\products.json");
                    await using var productDataStream = File.OpenRead(productDataPath);
                    var products = await JsonSerializer.DeserializeAsync<List<Product>>(productDataStream);

                    if (products != null && products.Any())
                    {
                        await _dbContext.Products.AddRangeAsync(products);
                    }
                }

                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Go to Error Page
            }
        }
    }
}
