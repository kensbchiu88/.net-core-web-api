using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.PublicApi.Models
{
    public class ApiRequest
    {
        [JsonProperty("Hwd")]
        public string Hwd { get; set; }
        [Required]
        [JsonProperty("Indicator")]
        public string Indicator { get; set; }
        [Required]
        [JsonProperty("SerializeData")]
        public string SerializeData { get; set; }

    }
}
