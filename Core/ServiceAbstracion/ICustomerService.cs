using Shared.DataTransferObjects.IdentityDTOs;
using Shared.DataTransferObjects.OrderDTOs;
namespace ServiceAbstracion
{
    public interface ICustomerService
    {
        Task<IReadOnlyList<CustomerDTO>> GetAllCustomersAsync();
        Task<CustomerDTO> GetCustomerByIdAsync(string userId);
        Task<IReadOnlyList<OrderToReturnDTO>> GetCustomerOrdersAsync(string userId);
        Task<CustomerStatsDTO> GetCustomerStatsAsync(string userId);
        Task DeleteCustomerAsync(string userId);
    }
}