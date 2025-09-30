using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class ProductQueryParams
    {
        public const int DefaultPageSize = 5;
        public const int MaxPageSize = 10;
        public int? BrandId { get; set; }

        public int? TypeId { get; set; }

        public ProductSortingOption SortingOption { get; set; }

        public string? SearchValue { get; set; }

        public int PageIndex { get; set; } = 1;  

        private int pageSize = DefaultPageSize;

        public int  PageSize
        {
            get => PageSize;
            set => PageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
