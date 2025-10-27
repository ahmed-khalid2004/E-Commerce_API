using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class CacheRepository(IConnectionMultiplexer connection) : ICacheRepository
    {
        readonly IDatabase _database = connection.GetDatabase();
        public async Task<string?> GetAsync(string Cachekey)
        {
            var Cachevalue =  await _database.StringGetAsync(Cachekey);
            return Cachevalue.IsNullOrEmpty ? null : Cachevalue.ToString();
        }

        public async Task SetAsync(string Cachekey, string CacheValue, TimeSpan timeToLive)
        {
            await _database.StringSetAsync(Cachekey, CacheValue, timeToLive);
        }
    }
}
