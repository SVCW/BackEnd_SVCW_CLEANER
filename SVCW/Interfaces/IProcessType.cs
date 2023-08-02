using SVCW.DTOs.ProcessTypes;
using SVCW.DTOs.ReportTypes;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IProcessType
    {
        Task<ProcessType> GetProcessTypeById(string? processTypeId);
        Task<ProcessType> SearchByNameProcessType(string? processTypeName);
        Task<List<ProcessType>> GetAllProcessType();
        Task<bool> UpdateProcessType(ProcessTypeDTO upProcessType);
        Task<bool> DeleteProcessType(List<string> processTypeId);
        Task<bool> InsertProcessType(ProcessTypeDTO processType);
    }
}
