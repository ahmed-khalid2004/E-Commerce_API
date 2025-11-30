using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.BasketModule
{
    public class CustomerBasket
    {
        public string Id { get; set; } // GUID : Created From Client [Frontend]

        public ICollection<BasketItem> Items { get; set; } = [];

        public string? clientSecret { get; set; }

        public string? paymentIntentId { get; set; }

        public int? deliveryMethodId { get; set; }

        public decimal? shippingPrice { get; set; }
    }
}
