using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PolarBearEapApi.Models
{
    public class ApiResponse
    {

        //[JsonPropertyName("Cmd")]
        //public int Cmd { get; set; }

        //[JsonPropertyName("Hwd")]
        //public string? Hwd { get; set; }

        [JsonPropertyName("Indicator")]
        [Required]
        public string Indicator { get; set; }

        [JsonPropertyName("SerializeData")]
        [Required]
        public string SerializeData { get; set; }

        [JsonPropertyName("Display")]
        public string? Display {  get; set; }

        //[JsonPropertyName("BindInfo")]
        //public string? BindInfo { get; set; }
    }
}
