using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.ProductModuleDTOs
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string PictureUrl { get; set; } = default!;
        public decimal Price { get; set; }
        public string ProductBrand { get; set; } = default!;
        public string ProductType { get; set; } = default!;
    }
}
