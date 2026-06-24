using System.Text.Json.Serialization;

namespace Shared
{
    public class ProductQueryParams
    {
        public const int DefaultPageSize = 5;
        public const int MaxPageSize = 10;

        [JsonPropertyName("brandId")]
        public int? BrandId { get; set; }

        [JsonPropertyName("subCategoryId")]
        public int? SubCategoryId { get; set; }

        [JsonPropertyName("categoryId")]
        public int? CategoryId { get; set; }

        [JsonPropertyName("sort")]
        public ProductSortingOption sort { get; set; }

        [JsonPropertyName("search")]
        public string? search { get; set; }

        [JsonPropertyName("pageNumber")]
        public int PageNumber { get; set; } = 1;

        private int _pageSize = DefaultPageSize;

        [JsonPropertyName("pageSize")]
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}