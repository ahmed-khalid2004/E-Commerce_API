using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.OrderModule
{
    public class Order : BaseEntity<Guid>
    {
        public Order()
        {
           
        }

        public Order(string userEmail, OrderAddress address, DeliveryMethod deliveryMethod, decimal subtotal, ICollection<OrderItem> items, string paymentIntentId)
        {
            BuyerEmail = userEmail;
            shipToAddress = address;
            DeliveryMethod = deliveryMethod;
            Subtotal = subtotal;
            Items = items;
            PaymentIntentId = paymentIntentId;
        }

        public string BuyerEmail { get; set; } = default!;

        public OrderAddress shipToAddress { get; set; } = default!;

        public DeliveryMethod DeliveryMethod { get; set; } = default!;

        public decimal Subtotal { get; set; }

        public ICollection<OrderItem> Items { get; set; } = [];

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.Now;
        public OrderStatus Status { get; set; }
        public int DeliveryMethodId { get; set; } //FK

        public decimal GetTotal() => Subtotal + DeliveryMethod.Cost;

        public string PaymentIntentId { get; set; } 
    }
}
