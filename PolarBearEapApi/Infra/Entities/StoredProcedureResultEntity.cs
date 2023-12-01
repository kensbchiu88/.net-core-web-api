using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.Infra.Entities
{
    public class StoredProcedureResultEntity
    {

        [Key]
        public string? Result { get; set; }
    }
}
