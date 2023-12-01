namespace PolarBearEapApi.ApplicationCore.Interfaces
{
    /** 抓取設定檔資料並Cache到MemoryCache的Service */
    public interface IConfigCacheService
    {
        public string GetValue(string key);
    }
}
