using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.Models
{
    public class ApiRequest
    {
        [JsonProperty("Hwd", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string Hwd { get; set; }
        [Required]
        [JsonProperty("Indicator", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string Indicator { get; set; }
        [Required]
        [JsonProperty("SerializeData", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string SerializeData { get; set; }

    }
}
