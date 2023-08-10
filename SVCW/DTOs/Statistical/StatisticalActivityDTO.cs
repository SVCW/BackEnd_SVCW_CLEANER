namespace SVCW.DTOs.Statistical
{
    public class StatisticalActivityDTO
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int count { get; set; }
        public int? numberLike { get; set; }
        public int? numberJoin { get; set; }
        public int? numberReport { get; set; }
        public int? numberComment { get; set; }
        public int? numberDonate { get; set; }
    }
}
