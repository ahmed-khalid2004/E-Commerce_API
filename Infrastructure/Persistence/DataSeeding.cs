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
            => Path.Combine(
                Directory.GetCurrentDirectory(),
                @"..\Infrastructure\Persistence\Data\DataSeed",
                fileName);

        // ── Entry points ──────────────────────────────────────────────────────

        public async Task DataSeedAsync()
        {
            // Apply pending migrations for BOTH contexts against the same DB
            await ApplyPendingMigrationsAsync();

            await SeedBrandsAsync();
            await SeedTypesAsync();
            await SeedProductsAsync();
            await SeedCategoriesAsync();
            await SeedProductCategoriesAsync();
            await SeedDeliveryMethodsAsync();
        }

        public async Task IdentityDataSeedAsync()
        {
            // Apply identity migrations
            var identityPending = await _identityDbContext.Database.GetPendingMigrationsAsync();
            if (identityPending.Any())
                await _identityDbContext.Database.MigrateAsync();

            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        // ── Phase 0 — Migrations ──────────────────────────────────────────────

        private async Task ApplyPendingMigrationsAsync()
        {
            var pending = await _dbContext.Database.GetPendingMigrationsAsync();
            if (pending.Any())
                await _dbContext.Database.MigrateAsync();
        }

        // ── Phase 1 — Brands ──────────────────────────────────────────────────

        private async Task SeedBrandsAsync()
        {
            var seedBrands = await DeserializeAsync<List<ProductBrand>>("brands.json");
            if (seedBrands is null || !seedBrands.Any()) return;

            var existingNames = await _dbContext.ProductBrands
                .Select(b => b.Name).ToHashSetAsync();

            var toInsert = seedBrands
                .Where(b => !existingNames.Contains(b.Name)).ToList();

            if (!toInsert.Any()) return;
            await _dbContext.ProductBrands.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ── Phase 2 — Types ───────────────────────────────────────────────────

        private async Task SeedTypesAsync()
        {
            var seedTypes = await DeserializeAsync<List<ProductType>>("types.json");
            if (seedTypes is null || !seedTypes.Any()) return;

            var existingNames = await _dbContext.ProductTypes
                .Select(t => t.Name).ToHashSetAsync();

            var toInsert = seedTypes
                .Where(t => !existingNames.Contains(t.Name)).ToList();

            if (!toInsert.Any()) return;
            await _dbContext.ProductTypes.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ── Phase 3 — Products ────────────────────────────────────────────────

        private async Task SeedProductsAsync()
        {
            var seedDtos = await DeserializeAsync<List<ProductSeedDto>>("products.json");
            if (seedDtos is null || !seedDtos.Any()) return;

            var brandsByName = await _dbContext.ProductBrands
                .ToDictionaryAsync(b => b.Name, b => b.Id);
            var typesByName = await _dbContext.ProductTypes
                .ToDictionaryAsync(t => t.Name, t => t.Id);
            var existingNames = await _dbContext.Products
                .Select(p => p.Name).ToHashSetAsync();

            var toInsert = new List<Product>();
            foreach (var dto in seedDtos)
            {
                if (existingNames.Contains(dto.Name)) continue;

                if (!brandsByName.TryGetValue(dto.BrandName, out var brandId))
                    throw new InvalidOperationException(
                        $"Seed error: Brand '{dto.BrandName}' not found for product '{dto.Name}'.");

                if (!typesByName.TryGetValue(dto.TypeName, out var typeId))
                    throw new InvalidOperationException(
                        $"Seed error: Type '{dto.TypeName}' not found for product '{dto.Name}'.");

                toInsert.Add(new Product
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    PictureUrl = dto.PictureUrl,
                    Price = dto.Price,
                    BrandId = brandId,
                    TypeId = typeId
                });
            }

            if (!toInsert.Any()) return;
            await _dbContext.Products.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ── Phase 4 — Categories ──────────────────────────────────────────────

        private async Task SeedCategoriesAsync()
        {
            var seedCategories = await DeserializeAsync<List<Category>>("categories.json");
            if (seedCategories is null || !seedCategories.Any()) return;

            var existingNames = await _dbContext.Categories
                .Select(c => c.Name).ToHashSetAsync();

            var toInsert = seedCategories
                .Where(c => !existingNames.Contains(c.Name)).ToList();

            if (!toInsert.Any()) return;
            await _dbContext.Categories.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ── Phase 5 — ProductCategories ───────────────────────────────────────

        private async Task SeedProductCategoriesAsync()
        {
            var seedLinks = await DeserializeAsync<List<ProductCategorySeedDto>>("product_categories.json");
            if (seedLinks is null || !seedLinks.Any()) return;

            var productIdByName = await _dbContext.Products
                .ToDictionaryAsync(p => p.Name, p => p.Id);
            var categoryIdByName = await _dbContext.Categories
                .ToDictionaryAsync(c => c.Name, c => c.Id);

            var existingPairSet = (await _dbContext.ProductCategories
                .Select(pc => new { pc.ProductId, pc.CategoryId })
                .ToListAsync())
                .Select(pc => (pc.ProductId, pc.CategoryId))
                .ToHashSet();

            var toInsert = new List<ProductCategory>();
            foreach (var link in seedLinks)
            {
                if (!productIdByName.TryGetValue(link.ProductName, out var productId))
                    throw new InvalidOperationException(
                        $"Seed error: Product '{link.ProductName}' not found.");

                if (!categoryIdByName.TryGetValue(link.CategoryName, out var categoryId))
                    throw new InvalidOperationException(
                        $"Seed error: Category '{link.CategoryName}' not found.");

                var pair = (productId, categoryId);
                if (existingPairSet.Contains(pair)) continue;

                toInsert.Add(new ProductCategory { ProductId = productId, CategoryId = categoryId });
                existingPairSet.Add(pair);
            }

            if (!toInsert.Any()) return;
            await _dbContext.ProductCategories.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ── Phase 6 — DeliveryMethods ─────────────────────────────────────────

        private async Task SeedDeliveryMethodsAsync()
        {
            var seedMethods = await DeserializeAsync<List<DeliveryMethod>>("delivery.json");
            if (seedMethods is null || !seedMethods.Any()) return;

            var existingNames = await _dbContext.DeliveryMethods
                .Select(d => d.ShortName).ToHashSetAsync();

            var toInsert = seedMethods
                .Where(d => !existingNames.Contains(d.ShortName)).ToList();

            if (!toInsert.Any()) return;
            await _dbContext.DeliveryMethods.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ── Identity seeding ──────────────────────────────────────────────────

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

        // ── Helpers ───────────────────────────────────────────────────────────

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