using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.OrderModule
{
    public enum OrderStatus
    {
        Pending = 0,
        PaymentReceived = 1,
        PaymentFailed = 2,
        Confirmed = 3,
        Shipped = 4,
        Delivered = 5,
        Cancelled = 6
    }
}
