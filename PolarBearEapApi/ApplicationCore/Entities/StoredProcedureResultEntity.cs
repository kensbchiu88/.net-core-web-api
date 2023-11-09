using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Entities
{
    public class StoredProcedureResultEntity
    {

        [Key]
        public string? Result { get; set; }
    }
}
