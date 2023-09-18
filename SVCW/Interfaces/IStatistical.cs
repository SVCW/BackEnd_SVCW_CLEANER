using SVCW.DTOs.Statistical;

namespace SVCW.Interfaces
{
    public interface IStatistical
    {
        Task<StatisticalUserDonateDTO> get(string userId, DateTime start, DateTime end);
        Task<StatisticalActivityDTO> getActivityBytime(DateTime start, DateTime end);
        Task<StatisticalDonateDTO> getDonateByTime(DateTime start, DateTime end);
        Task<StatisticalAdminDTO> getByTime(DateTime start, DateTime end);
    }
}
