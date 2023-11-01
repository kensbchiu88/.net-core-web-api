using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace PolarBearEapApi.ApplicationCore.Entities
{
    [Table("learn_file_alter_warning")]
    public class LearnFileAlterWarningEntity
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }
        [Column("file_path")]
        public string? FilePath { get; set; }
        [Column("equipment")]
        public string? Equipment { get; set; }
        [Column("alter_time")]
        public DateTime AlterTime { get; set; }
        [Column("create_time")]
        public DateTime? CreateTime { get; set; }
    }
}
