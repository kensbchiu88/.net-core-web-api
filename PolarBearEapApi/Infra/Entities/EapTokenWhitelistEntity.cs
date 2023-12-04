using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.Infra.Entities
{
    [Table("eap_token_whitelist")]
    public class EapTokenWhitelistEntity
    {
        [Key] // 指定Id为主键
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("username")]
        public string UserName { get; set; }
    }
}
