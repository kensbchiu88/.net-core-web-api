using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.PublicApi.Models
{
    public class MesCommandRequest
    {
        public string Hwd { get; set; }
        public string SerializeData { get; set; }
    }
}
