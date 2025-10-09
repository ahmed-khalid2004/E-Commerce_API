using DomainLayer.Contracts;
using DomainLayer.Models.IdentityModule;
using DomainLayer.Models.ProductModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Persistence
{
    public class DataSeeding(StoreDbContext _dbContext,
    UserManager<ApplicationUser> _userManager,
        RoleManager<IdentityRole> _roleManager,
        StoreIdentityDbContext _identityDbContext) : IDataSeeding
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

        public async Task IdentityDataSeedAsync()
        {
            try
            {
                if (!_roleManager.Roles.Any())
                {

                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                    await _roleManager.CreateAsync(new IdentityRole("SuperAdmin"));

                }
                if (!_userManager.Users.Any())
                {
                    var User01 = new ApplicationUser()
                    {
                        DisplayName = "Mohamed Tarek",
                        Email = "Mohamed@gmail.com",
                        UserName = "MohamedTarek",
                        PhoneNumber = "0123456789"
                    };
                    var User02 = new ApplicationUser()
                    {
                        DisplayName = "Salma Mohamed",
                        Email = "Salma@gmail.com",
                        UserName = "SalmaMohamed",
                        PhoneNumber = "0123456789"
                    };
                    await _userManager.CreateAsync(User01, "P@ssw0rd");
                    await _userManager.CreateAsync(User02, "P@ssw0rd");

                    await _userManager.AddToRoleAsync(User01, "Admin");
                    await _userManager.AddToRoleAsync(User02, "SuperAdmin");
                }
                await _identityDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                // Go to Error Page
            }
        }
    }
}
