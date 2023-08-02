using SVCW.DTOs.Process;
using SVCW.DTOs.ProcessTypes;
using SVCW.Models;
using System.Threading.Tasks;

namespace SVCW.Interfaces
{
    public interface IProcess
    {
        Task<Process> GetProcessById(string? processId);
        Task<List<Process>> SearchByTitleProcess(string? processTitle);
        Task<List<Process>> GetAllProcess();
        Task<List<Process>> GetProcessForActivity(string activityId);
        Task<Process> UpdateProcess(updateProcessDTO upProcess);
        Task<bool> DeleteProcess(string processId);
        Task<Process> InsertProcess(CreateProcessDTO process);
        Task<List<Process>> InsertProcessList(List<CreateProcessDTO> process);
    }
}
