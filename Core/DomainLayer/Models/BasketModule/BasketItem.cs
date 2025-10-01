using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.BasketModule
{
    public class BasketItem
    {
        public string Id { get; set; } // GUID : Created From Client [Frontend]

        public ICollection<BasketItem> Items { get; set; } = [];
    }
}
