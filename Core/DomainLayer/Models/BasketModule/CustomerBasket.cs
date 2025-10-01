using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.BasketModule
{
    public class CustomerBasket
    {
        public int Id { get; set; }

        public string ProductName { get; set; } = default!;

        public string PictureUrl { get; set; } = default!;

        public int Quantity { get; set; }

        public decimal Price { get; set; }
    }
}
