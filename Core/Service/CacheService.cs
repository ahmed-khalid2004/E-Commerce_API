using DomainLayer.Contracts;
using ServiceAbstracion;
using System.Text.Json;

namespace Service
{
    public class CacheService(ICacheRepository cacheRepository) : ICacheService
    {
        public async Task<string?> GetAsync(string Cachekey) => await cacheRepository.GetAsync(Cachekey);

        public async Task SetAsync(string Cachekey, object CacheValue, TimeSpan timeToLive)
        {
          var serializedValue = JsonSerializer.Serialize(CacheValue);
            await cacheRepository.SetAsync(Cachekey, serializedValue, timeToLive);
        }
    }
}
