using DomainLayer.Contracts;
using DomainLayer.Models.BasketModule;
using System.Text.Json;

namespace Persistence.Repositories
{
    public class BasketRepository(IRedisClient redis) : IBaseketRepository
    {
        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan? TimeToLive = null)
        {
            var jsonBasket = JsonSerializer.Serialize(basket);
            await redis.SetAsync(basket.Id, jsonBasket, TimeToLive ?? TimeSpan.FromDays(30));
            return await GetBasketASync(basket.Id);
        }

        public async Task<bool> DeleteBasketASync(string id)
        {
            await redis.DeleteAsync(id);
            return true;
        }

        public async Task<CustomerBasket?> GetBasketASync(string Key)
        {
            var basket = await redis.GetAsync(Key);
            return basket is null ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);
        }
    }
}