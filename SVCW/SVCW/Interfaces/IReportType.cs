using SVCW.DTOs.Achivements;
using SVCW.DTOs.ReportTypes;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IReportType
    {
        Task<ReportType> GetReportTypeById(string? reportTypeId);
        Task<ReportType> SearchByNameReportType(string? reportTypeName);
        Task<List<ReportType>> GetAllReportTypes();
        Task<bool> UpdateReportType(ReportTypeDTO upReportType);
        Task<bool> DeleteReportType(List<string> reportTypeId);
        Task<bool> InsertReportType(ReportTypeDTO reportType);
    }
}
