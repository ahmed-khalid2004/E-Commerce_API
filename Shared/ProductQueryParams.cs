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

        public ProductSortingOption sort { get; set; }

        public string? search { get; set; }

        public int PageNumber { get; set; } = 1;  

        private int pageSize = DefaultPageSize;

        public int  PageSize
        {   
            get => pageSize;
            set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
    }
}
