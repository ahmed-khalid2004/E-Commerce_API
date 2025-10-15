using Shared.DataTransferObjects.IdentityDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects.OrderDTOs
{
    public class OrderToReturnDTO
    {
        public Guid Id { get; set; }
        public string UserEmail { get; set; } = default!;

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;

        public AddressDTO Address { get; set; } = default!;

        public string DeliveryMethod { get; set; } = default!;

        public string OrderStatus { get; set; } = default!;

        public ICollection<OrderItemDTO> Items { get; set; } = [];

        public decimal Subtotal { get; set; }

        public decimal Total { get; set; }
    }
}
