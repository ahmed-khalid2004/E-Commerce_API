namespace DomainLayer.Contracts
{
    public interface IRedisClient
    {
        Task<string?> GetAsync(string key);
        Task SetAsync(string key, string value, TimeSpan? ttl = null);
        Task DeleteAsync(string key);
        Task<List<string>> ScanKeysAsync(string pattern);
    }
}