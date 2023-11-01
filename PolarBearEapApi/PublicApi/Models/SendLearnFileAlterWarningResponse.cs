using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PolarBearEapApi.PublicApi.Models
{
    public class SendLearnFileAlterWarningResponse
    {
        [JsonPropertyName("Result")]
        public string Result { get; set; }

        [JsonPropertyName("Display")]
        public string? Display { get; set; }
    }
}
