namespace ServiceAbstracion
{
    public interface ICacheService
    {
        Task<string?> GetAsync(string cacheKey);
        Task SetAsync(string cacheKey, object cacheValue, TimeSpan timeToLive);
        Task RemoveByPrefixAsync(string keyPrefix);
    }
}