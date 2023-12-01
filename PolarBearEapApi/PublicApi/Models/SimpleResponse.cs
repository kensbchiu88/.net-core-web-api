using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PolarBearEapApi.PublicApi.Models
{
    /** General Response Model */
    public class SimpleResponse<T>
    {
        [JsonPropertyName("Result")]

        public string Result { get; set; }
        [JsonPropertyName("Data")]
        [Required]
        public T? Data { get; set; }

        [JsonPropertyName("Message")]
        [Required]
        public string? Message { get; set; }
    }
}
