using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.BasketModule;
using ServiceAbstracion;
using Shared.DataTransferObjects.BasketModuleDTOs;

namespace Service
{
    public class BasketService(IBaseketRepository _basketRepository, IMapper _mapper) : IBasketService
    {
        public async Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket, string userId)
        {
            // Always force the basket Id to the authenticated user's Id.
            // The frontend does not send an Id at all (it's nullable in the DTO);
            // even if it did, this overwrite makes it impossible to target another user's basket.
            basket.Id = userId;

            var customerBasket = _mapper.Map<BasketDTO, CustomerBasket>(basket);
            var saved = await _basketRepository.CreateOrUpdateBasketAsync(customerBasket);

            if (saved is null)
                throw new Exception("Can Not Create Or Update Basket Now, Try Again Later");

            return await GetBasketASync(userId);
        }

        public async Task<bool> DeleteBasketASync(string userId)
            => await _basketRepository.DeleteBasketASync(userId);

        public async Task<BasketDTO> GetBasketASync(string userId)
        {
            var basket = await _basketRepository.GetBasketASync(userId)
                ?? throw new BasketNotFoundException(userId);

            return _mapper.Map<CustomerBasket, BasketDTO>(basket);
        }
    }
}