using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.PublicApi.Models
{
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
