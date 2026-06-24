using DomainLayer.Contracts;
using DomainLayer.Models.IdentityModule;
using DomainLayer.Models.ProductModule;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Persistence.Data;
using Persistence.Data.DataSeed;
using Persistence.Identity;
using System.Text.Json;

namespace Persistence
{
    public class DataSeeding(
        StoreDbContext _dbContext,
        UserManager<ApplicationUser> _userManager,
        RoleManager<IdentityRole> _roleManager,
        StoreIdentityDbContext _identityDbContext) : IDataSeeding
    {
        private static string SeedPath(string fileName)
            => Path.Combine(AppContext.BaseDirectory, "DataSeed", fileName);

        public async Task DataSeedAsync()
        {
            await ApplyPendingMigrationsAsync();

            await SeedBrandsAsync();
            await SeedCategoriesAsync();        // ← لازم تيجي قبل SubCategories دلوقتي
            await SeedSubCategoriesAsync();      // ← كانت SeedTypesAsync
            await SeedProductsAsync();
            await SeedDeliveryMethodsAsync();
            // SeedProductCategoriesAsync اتشالت بالكامل — مفيش Many-to-Many تاني
        }

        public async Task IdentityDataSeedAsync()
        {
            var identityPending = await _identityDbContext.Database.GetPendingMigrationsAsync();
            if (identityPending.Any())
                await _identityDbContext.Database.MigrateAsync();

            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        private async Task ApplyPendingMigrationsAsync()
        {
            var pending = await _dbContext.Database.GetPendingMigrationsAsync();
            if (pending.Any())
                await _dbContext.Database.MigrateAsync();
        }

        private async Task SeedBrandsAsync()
        {
            var seedBrands = await DeserializeAsync<List<ProductBrand>>("brands.json");
            if (seedBrands is null || !seedBrands.Any()) return;

            var existingNames = await _dbContext.ProductBrands.Select(b => b.Name).ToHashSetAsync();
            var toInsert = seedBrands.Where(b => !existingNames.Contains(b.Name)).ToList();

            if (!toInsert.Any()) return;
            await _dbContext.ProductBrands.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedCategoriesAsync()
        {
            var seedCategories = await DeserializeAsync<List<Category>>("categories.json");
            if (seedCategories is null || !seedCategories.Any()) return;

            var existingNames = await _dbContext.Categories.Select(c => c.Name).ToHashSetAsync();
            var toInsert = seedCategories.Where(c => !existingNames.Contains(c.Name)).ToList();

            if (!toInsert.Any()) return;
            await _dbContext.Categories.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ── Was SeedTypesAsync — now resolves CategoryId from CategoryName ────
        private async Task SeedSubCategoriesAsync()
        {
            var seedSubCategories = await DeserializeAsync<List<SubCategorySeedDto>>("subcategories.json");
            if (seedSubCategories is null || !seedSubCategories.Any()) return;

            var categoriesByName = await _dbContext.Categories
                .ToDictionaryAsync(c => c.Name, c => c.Id);
            var existingNames = await _dbContext.SubCategories
                .Select(s => s.Name).ToHashSetAsync();

            var toInsert = new List<SubCategory>();
            foreach (var dto in seedSubCategories)
            {
                if (existingNames.Contains(dto.Name)) continue;

                if (!categoriesByName.TryGetValue(dto.CategoryName, out var categoryId))
                    throw new InvalidOperationException(
                        $"Seed error: Category '{dto.CategoryName}' not found for subcategory '{dto.Name}'.");

                toInsert.Add(new SubCategory { Name = dto.Name, CategoryId = categoryId });
            }

            if (!toInsert.Any()) return;
            await _dbContext.SubCategories.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedProductsAsync()
        {
            var seedDtos = await DeserializeAsync<List<ProductSeedDto>>("products.json");
            if (seedDtos is null || !seedDtos.Any()) return;

            var brandsByName = await _dbContext.ProductBrands.ToDictionaryAsync(b => b.Name, b => b.Id);
            var subCategoriesByName = await _dbContext.SubCategories.ToDictionaryAsync(s => s.Name, s => s.Id);
            var existingNames = await _dbContext.Products.Select(p => p.Name).ToHashSetAsync();

            var toInsert = new List<Product>();
            foreach (var dto in seedDtos)
            {
                if (existingNames.Contains(dto.Name)) continue;

                if (!brandsByName.TryGetValue(dto.BrandName, out var brandId))
                    throw new InvalidOperationException($"Seed error: Brand '{dto.BrandName}' not found for product '{dto.Name}'.");

                if (!subCategoriesByName.TryGetValue(dto.SubCategoryName, out var subCategoryId))
                    throw new InvalidOperationException($"Seed error: SubCategory '{dto.SubCategoryName}' not found for product '{dto.Name}'.");

                toInsert.Add(new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    PictureUrl = dto.PictureUrl,
                    Price = dto.Price,
                    BrandId = brandId,
                    SubCategoryId = subCategoryId
                });
            }

            if (!toInsert.Any()) return;
            await _dbContext.Products.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedDeliveryMethodsAsync()
        {
            var seedMethods = await DeserializeAsync<List<DeliveryMethod>>("delivery.json");
            if (seedMethods is null || !seedMethods.Any()) return;

            var existingNames = await _dbContext.DeliveryMethods.Select(d => d.ShortName).ToHashSetAsync();
            var toInsert = seedMethods.Where(d => !existingNames.Contains(d.ShortName)).ToList();

            if (!toInsert.Any()) return;
            await _dbContext.DeliveryMethods.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        private async Task SeedRolesAsync()
        {
            foreach (var role in new[] { "Admin", "SuperAdmin" })
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
        }

        private async Task SeedUsersAsync()
        {
            var seedUsers = new[]
            {
                new { DisplayName = "Mohamed Tarek", Email = "Mohamed@gmail.com",
                      UserName = "MohamedTarek",  Phone = "0123456789", Role = "Admin"     },
                new { DisplayName = "Salma Mohamed", Email = "Salma@gmail.com",
                      UserName = "SalmaMohamed",  Phone = "0123456789", Role = "SuperAdmin" }
            };

            foreach (var seed in seedUsers)
            {
                if (await _userManager.FindByEmailAsync(seed.Email) is not null) continue;

                var user = new ApplicationUser
                {
                    DisplayName = seed.DisplayName,
                    Email = seed.Email,
                    UserName = seed.UserName,
                    PhoneNumber = seed.Phone
                };

                var result = await _userManager.CreateAsync(user, "P@ssw0rd");

                if (result.Succeeded)
                    await _userManager.AddToRoleAsync(user, seed.Role);
                else
                    throw new InvalidOperationException(
                        $"Failed to create seed user '{seed.Email}': " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        private static async Task<T?> DeserializeAsync<T>(string fileName)
        {
            var path = SeedPath(fileName);
            if (!File.Exists(path))
                throw new FileNotFoundException($"Seed file not found: '{path}'.");

            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<T>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}