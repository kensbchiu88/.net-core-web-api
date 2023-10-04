using Microsoft.Extensions.Caching.Memory;

namespace PolarBearEapApi.Commons
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public MemoryCacheService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        string ICacheService.TryGetValue(string cacheKey)
        {
            if (_memoryCache.TryGetValue(cacheKey, out string cachedValue))
            {
                return cachedValue;
            }

            var configKey = "AppCacheSettings:" + cacheKey;
            var configValue = _configuration[configKey];

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) // 設定過期時間
            };

            _memoryCache.Set(cacheKey, configValue, cacheEntryOptions);

            return configValue;
        }
    }
}
