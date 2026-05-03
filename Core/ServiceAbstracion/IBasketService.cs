using Shared.DataTransferObjects.BasketModuleDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstracion
{
    public interface IBasketService
    {
        Task<BasketDTO> GetBasketASync(string Key);

        Task<BasketDTO> CreateOrUpdateBasketAsync(BasketDTO basket);

        Task<bool> DeleteBasketASync(string Key);
    }
}
