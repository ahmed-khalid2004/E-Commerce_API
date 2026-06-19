using DomainLayer.Contracts;
using DomainLayer.Models.BasketModule;
using System.Collections.Concurrent;

namespace Persistence.Repositories
{
    /// <summary>
    /// Temporary in-memory replacement for BasketRepository (Redis).
    /// Used only while Redis is disabled. Data is NOT persistent across app restarts
    /// and is NOT shared across multiple server instances.
    /// </summary>
    public class InMemoryBasketRepository : IBaseketRepository
    {
        private static readonly ConcurrentDictionary<string, CustomerBasket> _store = new();

        public Task<CustomerBasket?> GetBasketASync(string Key)
        {
            _store.TryGetValue(Key, out var basket);
            return Task.FromResult(basket);
        }

        public Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan? TimeToLive = null)
        {
            _store[basket.Id] = basket;
            return Task.FromResult<CustomerBasket?>(basket);
        }

        public Task<bool> DeleteBasketASync(string id)
        {
            return Task.FromResult(_store.TryRemove(id, out _));
        }
    }
}