using DomainLayer.Contracts;
using StackExchange.Redis;

namespace Persistence.Repositories
{
    public class CacheRepository(IConnectionMultiplexer connection) : ICacheRepository
    {
        private readonly IDatabase _database = connection.GetDatabase();
        private readonly IServer _server = connection.GetServer(
            connection.GetEndPoints().First());

        public async Task<string?> GetAsync(string cacheKey)
        {
            var value = await _database.StringGetAsync(cacheKey);
            return value.IsNullOrEmpty ? null : value.ToString();
        }

        public async Task SetAsync(string cacheKey, string cacheValue, TimeSpan timeToLive)
            => await _database.StringSetAsync(cacheKey, cacheValue, timeToLive);

        public async Task RemoveByPrefixAsync(string keyPrefix)
        {
            var keys = _server.KeysAsync(pattern: $"{keyPrefix}*");
            await foreach (var key in keys)
                await _database.KeyDeleteAsync(key);
        }
    }
}