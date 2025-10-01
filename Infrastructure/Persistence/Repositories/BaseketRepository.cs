using DomainLayer.Models.BasketModule;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class BaseketRepository() : IBaseketRepository
    {
        public Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan? TimeToLive = null)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteBasketBSync(string id)
        {
            throw new NotImplementedException();
        }

        public Task<CustomerBasket?> GetBasketASync(string Key)
        {
            throw new NotImplementedException();
        }
    }
}
