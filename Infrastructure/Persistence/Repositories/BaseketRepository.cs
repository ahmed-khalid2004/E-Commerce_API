using DomainLayer.Models.BasketModule;
using StackExchange.Redis;
using System.Text.Json;

namespace Persistence.Repositories
{
    public class BaseketRepository(IConnectionMultiplexer connection) : IBaseketRepository
    {
        private readonly IDatabase _database = connection.GetDatabase();    
        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan? TimeToLive = null)
        {
            var JsonBasket = JsonSerializer.Serialize(basket);
            var IsCreatedOrUpdated = await _database.StringSetAsync(basket.Id, JsonBasket, TimeToLive ?? TimeSpan.FromDays(30));
            if (IsCreatedOrUpdated)
                return await GetBasketASync(basket.Id);
            else
                return null;
        }

        public async Task<bool> DeleteBasketASync(string id) => await _database.KeyDeleteAsync(id);

        public async Task<CustomerBasket?> GetBasketASync(string Key)
        {
           var Basket = await _database.StringGetAsync(Key);
            if (Basket.IsNullOrEmpty)   
                return null;
            else 
                return JsonSerializer.Deserialize<CustomerBasket>(Basket!);
        }
    }
}
