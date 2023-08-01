using SVCW.DTOs.UserSearchHistory;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface ISearchContent
    {
        Task<UserSearch> create(UserSearchHistoryDTO dto);
        Task<List<Activity>> recommendActivity(string userId);
        Task<List<Fanpage>> recommendFanpage(string userId);
    }
}
