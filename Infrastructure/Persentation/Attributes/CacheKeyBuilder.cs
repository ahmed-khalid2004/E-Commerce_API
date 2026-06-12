using Microsoft.AspNetCore.Http;
using System.Text;

namespace Presentation.Attributes
{
    /// <summary>
    /// Builds deterministic, order-independent cache keys from HTTP request parameters.
    ///
    /// Key contract:
    ///   - Parameters are sorted alphabetically by name (order-independent)
    ///   - Parameter names are lowercased
    ///   - Parameter values are lowercased and trimmed
    ///   - Null, empty, or whitespace-only values are excluded entirely
    ///   - Absence of a parameter == parameter with empty value (no key divergence)
    ///
    /// Format:
    ///   {path.toLowerCase()}|{key1}={val1}&{key2}={val2}
    ///
    /// Example:
    ///   /api/products|brandid=2&categoryid=3&pagenuber=1&pagesize=5&sort=priceasc&typeid=1
    ///
    /// ?typeId=1&categoryId=2  ==  ?categoryId=2&typeId=1  → same key ✔
    /// ?search=Boots           ==  ?search=boots           → same key ✔
    /// ?categoryId=            ==  (absent categoryId)     → same key ✔
    /// </summary>
    public static class CacheKeyBuilder
    {
        // Explicit allowlist of dimensions that form the cache key.
        // Add new filter dimensions here — nowhere else.
        private static readonly HashSet<string> _knownParams = new(StringComparer.OrdinalIgnoreCase)
        {
            "brandid",
            "typeid",
            "categoryid",      // forward-compatible: included now, ignored when absent
            "search",
            "sort",
            "pagenumber",
            "pagesize"
        };

        public static string Build(HttpRequest request)
        {
            var path = request.Path.Value?.ToLowerInvariant() ?? string.Empty;

            // Sort by normalized key, exclude empty values
            var normalizedParams = request.Query
                .Where(q => _knownParams.Contains(q.Key.ToLowerInvariant()))
                .Where(q => !string.IsNullOrWhiteSpace(q.Value))
                .OrderBy(q => q.Key.ToLowerInvariant())
                .Select(q =>
                    $"{q.Key.ToLowerInvariant()}=" +
                    $"{q.Value.ToString().Trim().ToLowerInvariant()}");

            var sb = new StringBuilder();
            sb.Append(path);
            sb.Append('|');
            sb.Append(string.Join('&', normalizedParams));

            return sb.ToString();
        }
    }
}