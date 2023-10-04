using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.Repository
{
    [Table("eap_token")]
    public class EapTokenEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("username")]
        public string? username { get; set; }
        [Column("line_code")]
        public string? LineCode { get; set; }
        [Column("section_code")]
        public string? SectionCode { get; set; }
        [Column("station_code")]
        public int? StationCode { get; set; }
        [Column("server_version")]
        public string? ServerVersion { get; set; }
        [Column("login_time")]
        public DateTime LoginTime { get; set; }
        [Column("bind_time")]
        public DateTime? BindTime { get; set; }

    }
}
