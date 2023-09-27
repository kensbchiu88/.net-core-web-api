using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace PolarBearEapApi.Models
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
        public string? Display {  get; set; }
        [JsonProperty("BindInfo", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? BindInfo { get; set; }
    }
}
