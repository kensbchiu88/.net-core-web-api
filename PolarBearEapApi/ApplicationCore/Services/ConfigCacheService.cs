using Microsoft.Extensions.Caching.Memory;
using PolarBearEapApi.ApplicationCore.Interfaces;

namespace PolarBearEapApi.ApplicationCore.Services
{
    /** 抓取設定檔資料並Cache到MemoryCache的Service */
    public class ConfigCacheService : IConfigCacheService
    {
        private readonly IConfiguration _configuration;
        private readonly IMemoryCache _memoryCache;

        public ConfigCacheService(IConfiguration configuration, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _memoryCache = memoryCache;
        }

        /** 抓取設定檔。若Cache中有資料則從Cache中抓，若Cache中沒資料則從設定檔中抓取，並存在Cache中 */
        string IConfigCacheService.GetValue(string cacheKey)
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
