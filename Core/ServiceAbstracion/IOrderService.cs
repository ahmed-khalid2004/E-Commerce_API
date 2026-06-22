using Shared.DataTransferObjects.OrderDTOs;

namespace ServiceAbstracion
{
    public interface IOrderService
    {
        Task<OrderToReturnDTO> CreateOrder(OrderDTO orderDto, string email, string userId);
        Task<IEnumerable<DeliveryMethodDTO>> GetDeliveryMethodsAsync();
        Task<IEnumerable<OrderToReturnDTO>> GetAllOrdersAsync(string email);
        Task<OrderToReturnDTO> GetOrderByIdAsync(Guid id, string email);

        // Admin
        Task<IReadOnlyList<OrderToReturnDTO>> GetAllOrdersForAdminAsync();
        Task<OrderToReturnDTO> UpdateOrderStatusAsync(Guid orderId, string newStatus);
    }
}