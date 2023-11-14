﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PolarBearEapApi.ApplicationCore.Entities
{
    [Table("equipment_temporary_data")]
    public class EquipmentTemporaryDataEntity
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
        [Column("data_key")]
        public string DataKey { get; set; }
        [Required]
        [Column("data_value")]
        public string DataValue { get; set; }
        [Required]
        [Column("create_time")]
        public DateTime CreateTime { get; set; }
    }
}
