namespace DomainLayer.Contracts
{
    public interface ICacheRepository
    {
        Task<string?> GetAsync(string cacheKey);
        Task SetAsync(string cacheKey, string cacheValue, TimeSpan timeToLive);
        Task RemoveByPrefixAsync(string keyPrefix);
    }
}