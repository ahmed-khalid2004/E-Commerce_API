using Shared.DataTransferObjects.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class OrderDTO
    {
        public string BasketId { get; set; } = default!;

        public int DeliveryMethodId { get; set; }

        public AddressDTO shipToAddress { get; set; } = default!;
    }
}
