using DomainLayer.Contracts;
using Persistence.Caching;

namespace Persistence.Repositories
{
    public class CacheRepository(IRedisClient redis) : ICacheRepository
    {
        public async Task<string?> GetAsync(string cacheKey)
            => await redis.GetAsync(cacheKey);

        public async Task SetAsync(string cacheKey, string cacheValue, TimeSpan timeToLive)
            => await redis.SetAsync(cacheKey, cacheValue, timeToLive);

        public async Task RemoveByPrefixAsync(string keyPrefix)
        {
            var keys = await redis.ScanKeysAsync($"{keyPrefix}*");
            foreach (var key in keys)
                await redis.DeleteAsync(key);
        }
    }
}