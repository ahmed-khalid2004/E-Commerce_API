using DomainLayer.Models.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts
{
    public interface IBaseketRepository
    {
        Task<CustomerBasket?> GetBasketASync(string Key);

        Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket,TimeSpan? TimeToLive = null);

        Task<bool> DeleteBasketBSync(string id);
    }
}
