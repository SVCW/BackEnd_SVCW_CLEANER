namespace SVCW.DTOs.Statistical
{
    public class StatisticalDonateDTO
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public int? numberCreateActivity { get; set; }
        public decimal total { get; set; }
        public decimal target { get; set; }
    }
}
