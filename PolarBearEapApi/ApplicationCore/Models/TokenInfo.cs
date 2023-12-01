namespace PolarBearEapApi.ApplicationCore.Models
{
    /** Token Aggregate Object */
    public class TokenInfo
    {
        public string Id { get; set; }
        public string? username { get; set; }
        public string? LineCode { get; set; }
        public string? SectionCode { get; set; }
        public int? StationCode { get; set; }
        public string? ServerVersion { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? BindTime { get; set; }
    }
}
