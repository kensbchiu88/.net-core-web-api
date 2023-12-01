using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace PolarBearEapApi.PublicApi.Models
{
    /** /api/v1/EapApi/SendLearnFileAlterWarning Request Model */
    public class SendLearnFileAlterWarningRequest
    {
        [JsonProperty("filePath", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? FilePath { get; set; }
        [JsonProperty("alterTime", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? AlterTime { get; set; }
        [JsonProperty("equipment", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string? Equipment { get; set; }
    }
}
