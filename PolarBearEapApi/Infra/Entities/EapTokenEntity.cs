using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApi.Infra.Entities
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
        [Column("card_time")]
        public DateTime? CardTime { get; set; }
        [Column("is_invalid")]
        public bool? IsInvalid { get; set; }

        public static TokenInfo ConvertToTokenInfo(EapTokenEntity source)
        {
            TokenInfo tokenInfo = new TokenInfo();
            tokenInfo.Id = source.Id.ToString();
            tokenInfo.username = source.username;
            tokenInfo.LineCode = source.LineCode;
            tokenInfo.SectionCode = source.SectionCode;
            tokenInfo.StationCode = source.StationCode;
            tokenInfo.LoginTime = source.LoginTime;
            tokenInfo.BindTime = source.BindTime;

            return tokenInfo;
        }

    }
}
