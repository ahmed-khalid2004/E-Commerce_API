using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.ProductModule
{
    public class ProductBrand : BaseEntity<int>
    {
        public string Name { get; set; } = default!;
        public ICollection<Product> Products { get; set; }
    }
}
