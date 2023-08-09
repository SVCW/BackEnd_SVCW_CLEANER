using SVCW.DTOs.Statistical;

namespace SVCW.Interfaces
{
    public interface IStatistical
    {
        Task<StatisticalUserDonateDTO> get(string userId, DateTime start, DateTime end);
    }
}
