namespace Shared
{
    public class ProductQueryParams
    {
        public const int DefaultPageSize = 5;
        public const int MaxPageSize = 10;

        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public int? CategoryId { get; set; }   // Phase 1: declared, unused in specs until Phase 4

        public ProductSortingOption sort { get; set; }
        public string? search { get; set; }

        public int PageNumber { get; set; } = 1;

        private int _pageSize = DefaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}