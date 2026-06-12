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
    /// <summary>
    /// Deterministic, idempotent data seeder.
    ///
    /// Design rules enforced here:
    ///  1. Name is the identity anchor — no hardcoded DB-generated IDs anywhere.
    ///  2. Strict dependency order: Brands → Types → Products → Categories → ProductCategories.
    ///  3. Each phase saves independently so failures are isolated and diagnosable.
    ///  4. Every insert is guarded by a Name-level existence check (not just Any()).
    ///  5. EF tracking is used deliberately: tracked entities are fetched once into
    ///     dictionaries keyed by Name, then reused — no double-tracking.
    /// </summary>
    public class DataSeeding(
        StoreDbContext _dbContext,
        UserManager<ApplicationUser> _userManager,
        RoleManager<IdentityRole> _roleManager,
        StoreIdentityDbContext _identityDbContext) : IDataSeeding
    {
        // ─── path helper ────────────────────────────────────────────────────────────
        private static string SeedPath(string fileName)
            => Path.Combine(
                Directory.GetCurrentDirectory(),
                @"..\Infrastructure\Persistence\Data\DataSeed",
                fileName);

        // ─── public entry points ─────────────────────────────────────────────────────

        public async Task DataSeedAsync()
        {
            await ApplyPendingMigrationsAsync();

            await SeedBrandsAsync();
            await SeedTypesAsync();
            await SeedProductsAsync();          // resolves BrandName / TypeName → FK
            await SeedCategoriesAsync();
            await SeedProductCategoriesAsync(); // resolves ProductName / CategoryName → FK
        }

        public async Task IdentityDataSeedAsync()
        {
            await SeedRolesAsync();
            await SeedUsersAsync();
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PHASE 0 — Migrations
        // ════════════════════════════════════════════════════════════════════════════

        private async Task ApplyPendingMigrationsAsync()
        {
            var pending = await _dbContext.Database.GetPendingMigrationsAsync();
            if (pending.Any())
                await _dbContext.Database.MigrateAsync();
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PHASE 1 — ProductBrands
        // ════════════════════════════════════════════════════════════════════════════

        private async Task SeedBrandsAsync()
        {
            var seedBrands = await DeserializeAsync<List<ProductBrand>>("brands.json");
            if (seedBrands is null || !seedBrands.Any()) return;

            // Load existing names into a HashSet for O(1) lookup
            var existingNames = await _dbContext.ProductBrands
                .Select(b => b.Name)
                .ToHashSetAsync();

            var toInsert = seedBrands
                .Where(b => !existingNames.Contains(b.Name))
                .ToList();

            if (!toInsert.Any()) return;

            await _dbContext.ProductBrands.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PHASE 2 — ProductTypes
        // ════════════════════════════════════════════════════════════════════════════

        private async Task SeedTypesAsync()
        {
            var seedTypes = await DeserializeAsync<List<ProductType>>("types.json");
            if (seedTypes is null || !seedTypes.Any()) return;

            var existingNames = await _dbContext.ProductTypes
                .Select(t => t.Name)
                .ToHashSetAsync();

            var toInsert = seedTypes
                .Where(t => !existingNames.Contains(t.Name))
                .ToList();

            if (!toInsert.Any()) return;

            await _dbContext.ProductTypes.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PHASE 3 — Products  (identity resolution: BrandName → BrandId, TypeName → TypeId)
        // ════════════════════════════════════════════════════════════════════════════

        private async Task SeedProductsAsync()
        {
            var seedDtos = await DeserializeAsync<List<ProductSeedDto>>("products.json");
            if (seedDtos is null || !seedDtos.Any()) return;

            // Build lookup dictionaries from already-saved (tracked) entities.
            // These are loaded AFTER SaveChanges in the brand/type phases,
            // so their DB-generated IDs are now available.
            var brandsByName = await _dbContext.ProductBrands
                .ToDictionaryAsync(b => b.Name, b => b.Id);

            var typesByName = await _dbContext.ProductTypes
                .ToDictionaryAsync(t => t.Name, t => t.Id);

            var existingProductNames = await _dbContext.Products
                .Select(p => p.Name)
                .ToHashSetAsync();

            var toInsert = new List<Product>();

            foreach (var dto in seedDtos)
            {
                if (existingProductNames.Contains(dto.Name))
                    continue; // already seeded — skip

                if (!brandsByName.TryGetValue(dto.BrandName, out var brandId))
                    throw new InvalidOperationException(
                        $"Seed error: Brand '{dto.BrandName}' not found for product '{dto.Name}'. " +
                        "Ensure brands.json contains this brand and SeedBrandsAsync ran first.");

                if (!typesByName.TryGetValue(dto.TypeName, out var typeId))
                    throw new InvalidOperationException(
                        $"Seed error: Type '{dto.TypeName}' not found for product '{dto.Name}'. " +
                        "Ensure types.json contains this type and SeedTypesAsync ran first.");

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

        // ════════════════════════════════════════════════════════════════════════════
        // PHASE 4 — Categories
        // ════════════════════════════════════════════════════════════════════════════

        private async Task SeedCategoriesAsync()
        {
            var seedCategories = await DeserializeAsync<List<Category>>("categories.json");
            if (seedCategories is null || !seedCategories.Any()) return;

            var existingNames = await _dbContext.Categories
                .Select(c => c.Name)
                .ToHashSetAsync();

            var toInsert = seedCategories
                .Where(c => !existingNames.Contains(c.Name))
                .ToList();

            if (!toInsert.Any()) return;

            await _dbContext.Categories.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════════════
        // PHASE 5 — ProductCategories  (identity resolution: ProductName + CategoryName)
        //
        // This is the core of the deterministic many-to-many seeding.
        //
        // WHY name-based resolution?
        //   IDs are assigned by the DB engine and may differ across environments.
        //   Relying on them in seed files creates silent FK mismatches.
        //   Names are stable, human-readable domain identifiers.
        //
        // WHY build a HashSet<(int,int)>?
        //   Checking existence in EF change tracker or DB one-by-one is O(N×M).
        //   A HashSet gives O(1) lookup per pair — safe and fast even for large seed sets.
        //
        // WHY throw on missing product/category names?
        //   Silent skipping would leave relationships partially built and hard to debug.
        //   Fail-fast surfaces the problem immediately with an actionable message.
        // ════════════════════════════════════════════════════════════════════════════

        private async Task SeedProductCategoriesAsync()
        {
            var seedLinks = await DeserializeAsync<List<ProductCategorySeedDto>>("product_categories.json");
            if (seedLinks is null || !seedLinks.Any()) return;

            // Build lookup dictionaries: Name → Id
            // These are loaded AFTER Products and Categories are saved, so IDs are real.
            var productIdByName = await _dbContext.Products
                .ToDictionaryAsync(p => p.Name, p => p.Id);

            var categoryIdByName = await _dbContext.Categories
                .ToDictionaryAsync(c => c.Name, c => c.Id);

            // Load existing pairs into a HashSet for O(1) duplicate detection.
            // This is the idempotency guard: we never insert a pair that already exists.
            var existingPairs = await _dbContext.ProductCategories
                .Select(pc => new { pc.ProductId, pc.CategoryId })
                .ToListAsync();

            var existingPairSet = existingPairs
                .Select(pc => (pc.ProductId, pc.CategoryId))
                .ToHashSet();

            var toInsert = new List<ProductCategory>();

            foreach (var link in seedLinks)
            {
                // Fail-fast: both sides must exist in the DB before we can link them.
                if (!productIdByName.TryGetValue(link.ProductName, out var productId))
                    throw new InvalidOperationException(
                        $"Seed error: Product '{link.ProductName}' not found in DB. " +
                        "Ensure products.json contains this product and SeedProductsAsync ran first.");

                if (!categoryIdByName.TryGetValue(link.CategoryName, out var categoryId))
                    throw new InvalidOperationException(
                        $"Seed error: Category '{link.CategoryName}' not found in DB. " +
                        "Ensure categories.json contains this category and SeedCategoriesAsync ran first.");

                var pair = (productId, categoryId);

                if (existingPairSet.Contains(pair))
                    continue; // already linked — idempotent skip

                toInsert.Add(new ProductCategory
                {
                    ProductId = productId,
                    CategoryId = categoryId
                });

                // Add to the in-memory set so duplicate entries within the seed
                // file itself are also caught (not just DB duplicates).
                existingPairSet.Add(pair);
            }

            if (!toInsert.Any()) return;

            await _dbContext.ProductCategories.AddRangeAsync(toInsert);
            await _dbContext.SaveChangesAsync();
        }

        // ════════════════════════════════════════════════════════════════════════════
        // IDENTITY SEEDING
        // ════════════════════════════════════════════════════════════════════════════

        private async Task SeedRolesAsync()
        {
            var roles = new[] { "Admin", "SuperAdmin" };
            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                    await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        private async Task SeedUsersAsync()
        {
            var seedUsers = new[]
            {
                new { DisplayName = "Mohamed Tarek",  Email = "Mohamed@gmail.com", UserName = "MohamedTarek",  Phone = "0123456789", Role = "Admin"      },
                new { DisplayName = "Salma Mohamed",  Email = "Salma@gmail.com",   UserName = "SalmaMohamed",  Phone = "0123456789", Role = "SuperAdmin"  }
            };

            foreach (var seed in seedUsers)
            {
                // Guard by email — stable, unique identifier
                if (await _userManager.FindByEmailAsync(seed.Email) is not null)
                    continue;

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

        // ════════════════════════════════════════════════════════════════════════════
        // HELPERS
        // ════════════════════════════════════════════════════════════════════════════

        private static async Task<T?> DeserializeAsync<T>(string fileName)
        {
            var path = SeedPath(fileName);

            if (!File.Exists(path))
                throw new FileNotFoundException(
                    $"Seed file not found: '{path}'. " +
                    "Ensure the file exists under Infrastructure/Persistence/Data/DataSeed/.");

            await using var stream = File.OpenRead(path);
            return await JsonSerializer.DeserializeAsync<T>(stream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}