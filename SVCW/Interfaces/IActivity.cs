﻿using SVCW.DTOs.Activities;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IActivity
    {
        Task<Activity> createActivity(ActivityCreateDTO dto);
        Task<Activity> updateActivity(ActivityUpdateDTO dto);
        Task<Activity> getById(string id);
        Task<List<Activity>> getAll(int pageSize, int PageLoad);
        Task<List<Activity>> getByTitle(string title);
        Task<List<Activity>> getForUser(int pageSize, int PageLoad);
        Task<List<Activity>> getActivityUser(string userId);
        Task<List<Activity>> getActivityFanpage(string fanpageId);
        Task<Activity> delete(string id);
        Task<Activity> deleteAdmin(string id);
        Task<bool> followActivity(string activityId, string userId);
        Task<bool> unFollowActivity(string activityId, string userId);
        Task<bool> joinActivity(string activityId, string userId);
        Task<bool> disJoinActivity(string activityId, string userId);
        Task<List<Activity>> getDataLoginPage();
        Task<List<Activity>> getActivityAfterEndDate();
        Task<List<Activity>> getActivityBeforeEndDate();
        Task<List<Activity>> getActivityBeforeStartDate();
    }
}