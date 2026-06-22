using Shared.DataTransferObjects.IdentityDTOs;

namespace ServiceAbstracion
{
    public interface ICustomerService
    {
        Task<IReadOnlyList<CustomerDTO>> GetAllCustomersAsync();
        Task<CustomerStatsDTO> GetCustomerStatsAsync(string userId);
        Task DeleteCustomerAsync(string userId);
    }
}
