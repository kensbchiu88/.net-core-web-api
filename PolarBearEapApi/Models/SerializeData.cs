using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.Models
{
    public class SerializeData
    {
        [Required]
        [JsonProperty("LineCode", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string LineCode { get; set; }
        [Required]
        [JsonProperty("SectionCode", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string SectionCode { get; set; }
        [Required]
        [JsonProperty("StationCode", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public int StationCode { get; set; }
        [Required]
        [JsonProperty("OPCategory", NamingStrategyType = typeof(SnakeCaseNamingStrategy))]
        public string OPCategory { get; set; }
    }
}
