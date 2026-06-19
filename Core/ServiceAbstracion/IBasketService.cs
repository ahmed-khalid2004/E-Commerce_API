using Shared.DataTransferObjects.BasketModuleDTOs;

namespace ServiceAbstracion
{
    public interface IBasketService
    {
        Task<BasketDTO> GetBasketASync(string userId);
        Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket, string userId);
        Task<bool> DeleteBasketASync(string userId);
    }
}