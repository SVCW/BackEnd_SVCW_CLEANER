using SVCW.DTOs.UserSearchHistory;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class UserSearchService : ISearchContent
    {
        private readonly SVCWContext _context;

        public UserSearchService(SVCWContext context)
        {
            _context = context;
        }
        public async Task<UserSearch> create(UserSearchHistoryDTO dto)
        {

            try
            {
                var userSearch = new UserSearch();
                userSearch.SearchContent = dto.SearchContent;
                userSearch.Datetime = DateTime.Now;
                userSearch.UserId = dto.userId;
                userSearch.UserSearchId = "UR" + Guid.NewGuid().ToString().Substring(0, 8);
                
                await this._context.UserSearch.AddAsync(userSearch);
                await this._context.SaveChangesAsync();
                return userSearch;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Task<List<Activity>> recommendActivity(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Fanpage>> recommendFanpage(string userId)
        {
            throw new NotImplementedException();
        }
    }
}
