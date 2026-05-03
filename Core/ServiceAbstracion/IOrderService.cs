using Shared.DataTransferObjects.OrderDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstracion
{
    public interface IOrderService
    {
        
        Task<OrderToReturnDTO> CreateOrder(OrderDTO orderDto,string Email);
        
        Task<IEnumerable<DeliveryMethodDTO>> GetDeliveryMethodsAsync();

        Task<IEnumerable<OrderToReturnDTO>> GetAllOrdersAsync(string Email);

        Task<OrderToReturnDTO> GetOrderByIdAsync(Guid Id);
    }
}
