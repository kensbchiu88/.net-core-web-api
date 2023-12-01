using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolarBearEapApi.Infra.Entities
{
    [Table("upload_report")]
    public class UploadReportEntity
    {
        [Key] // 指定Id为主键
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }
        [Required]
        [Column("line_code")]
        public string LineCode { get; set; }
        [Required]
        [Column("section_code")]
        public string SectionCode { get; set; }
        [Required]
        [Column("station_code")]
        public int StationCode { get; set; }
        [Required]
        [Column("sn")]
        public string Sn { get; set; }
        [Required]
        [Column("op_request_info")]
        public string OpRequestInfo { get; set; }
        [Required]
        [Column("upload_time")]
        public DateTime UploadTime { get; set; }
    }
}
