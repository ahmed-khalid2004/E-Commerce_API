using AutoMapper;
using DomainLayer.Contracts;
using DomainLayer.Exceptions;
using DomainLayer.Models.BasketModule;
using ServiceAbstracion;
using Shared.DataTransferObjects.BasketModuleDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class BasketService(IBaseketRepository _baseketRepository, IMapper _mapper) : IBasketService
    {
        public async Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket)
        {
            var CustomerBasket = _mapper.Map<BasketDTO, CustomerBasket>(basket);
            var CreateOrUpdateBasket = await _baseketRepository.CreateOrUpdateBasketAsync(CustomerBasket);
            if (CreateOrUpdateBasket != null)
                return await GetBasketASync(basket.Id);
            else
                throw new Exception("Can Not Create Or Update Basket Now , Try Again Later");
        }

        public async Task<bool> DeleteBasketASync(string Key) => await _baseketRepository.DeleteBasketASync(Key);

        public async Task<BasketDTO> GetBasketASync(string Key)
        {
            var Basket = await _baseketRepository.GetBasketASync(Key);
            if (Basket is not null)
                return _mapper.Map<CustomerBasket, BasketDTO>(Basket);
            else
                throw new BasketNotFoundException(Key);
        }
    }
}
