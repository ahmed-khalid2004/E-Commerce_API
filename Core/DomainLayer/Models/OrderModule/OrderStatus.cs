using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models.OrderModule
{
    public enum OrderStatus
    {
        pending = 0,
        PaymentRecevied = 1,
        PaymentFailed = 2
    }
}
