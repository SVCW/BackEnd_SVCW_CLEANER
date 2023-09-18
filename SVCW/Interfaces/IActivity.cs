using SVCW.DTOs.Activities;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IActivity
    {
        Task<Activity> createActivity(ActivityCreateDTO dto);
        Task<Activity> updateActivity(ActivityUpdateDTO dto);
        Task<Activity> getById(string id);
        Task<List<Activity>> getAll(int pageSize, int PageLoad);
        Task<List<Activity>> getByTitle(SearchDTO title);
        Task<List<Activity>> getForUser(int pageSize, int PageLoad);
        Task<List<Activity>> getActivityUser(string userId);
        Task<List<Activity>> getActivityFanpage(string fanpageId);
        Task<Activity> delete(string id);
        Task<Activity> deleteAdmin(string id);
        Task<Activity> activePending(string id);
        Task<Activity> reActive(string id);
        Task<Activity> rejectActivity(RejectActivityDTO dto);
        Task<Activity> quitActivity(QuitActivityDTO dto);
        Task<bool> followActivity(string activityId, string userId);
        Task<bool> unFollowActivity(string activityId, string userId);
        Task<bool> joinActivity(string activityId, string userId);
        Task<bool> disJoinActivity(string activityId, string userId);
        Task<List<Activity>> getDataLoginPage();
        Task<List<Activity>> getActivityPending();
        Task<List<Activity>> getActivityAfterEndDate();
        Task<List<Activity>> getActivityBeforeEndDate();
        Task<List<Activity>> getActivityBeforeStartDate();
        Task<List<Activity>> getActivityBeforeStartDateUser(string userId);
        Task<List<Activity>> getActivityReject(string userId);
        Task<List<Activity>> getActivityRejectAdmin();
        Task<List<Activity>> getActivityQuit(string userId);
        Task<List<Activity>> getActivityQuitAdmin();
        Task<SearchResultDTO> search(SearchDTO searchContent);
        Task<bool> checkIn(string activityId, string userId);
        Task<Activity> checkQR(string activityId);
    }
}
