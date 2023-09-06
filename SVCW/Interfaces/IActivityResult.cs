using SVCW.DTOs.ActivityResults;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IActivityResult
    {
        Task<ActivityResult> create(ActivityResultCreateDTO dto);
        Task<ActivityResult> update(ActivityResultUpdateDTO dto);
        Task<List<ActivityResult>> getAll();
        Task<List<ActivityResult>> getForActivity(string activityId);
        Task<ActivityResultDTO> getForActivityv2(string activityId);
    }
}
