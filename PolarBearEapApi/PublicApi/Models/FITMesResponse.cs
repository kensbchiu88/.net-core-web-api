using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace PolarBearEapApi.PublicApi.Models
{
    public class FITMesResponse
    {
        [JsonProperty("Result", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? Result { get; set; }
        [JsonProperty("ResultCoded", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? ResultCode { get; set; }
        [JsonProperty("MessageCode", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? MessageCode { get; set; }
        [JsonProperty("Display", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? Display { get; set; }
        [JsonProperty("BindInfo", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? BindInfo { get; set; }

        /** 檢查MES執行結果 */
        public static bool IsResultOk(string mesReturnJson)
        {
            if (string.IsNullOrWhiteSpace(mesReturnJson)) return false;

            FITMesResponse? fitMesResponse = JsonConvert.DeserializeObject<FITMesResponse>(mesReturnJson);
            if (fitMesResponse == null || string.IsNullOrEmpty(fitMesResponse.Result) || !fitMesResponse.Result.ToUpper().Equals("OK"))
            {
                return false;
            }
            return true;
        }
    }
}
