using Microsoft.EntityFrameworkCore;
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

        public async Task<List<Activity>> recommendActivity(string userId)
        {
            try
            {
                var li = new List<Activity>();
                var check = await this._context.UserSearch.Where(x => x.UserId.Equals(userId)).OrderByDescending(x => x.Datetime).ToListAsync();
                foreach (var activity in check)
                {
                    var recommend = this._context.Activity.Where(x => x.Title.Contains(activity.SearchContent))
                        .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null))
                        .ThenInclude(x => x.InverseReply.OrderByDescending(x => x.Datetime))
                            .ThenInclude(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Process.OrderBy(x => x.ProcessNo).Where(x => x.Status))
                        .ThenInclude(x => x.Media)
                    .Include(x => x.Donation)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount).OrderByDescending(x => x.CreateAt).Take(1);
                    foreach (var lix in recommend)
                    {
                        li.Add(lix);
                    }
                }
                return li;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Fanpage>> recommendFanpage(string userId)
        {
            try
            {
                var li = new List<Fanpage>();
                var check = await this._context.UserSearch.Where(x => x.UserId.Equals(userId)).OrderByDescending(x => x.Datetime).ToListAsync();
                foreach (var activity in check)
                {
                    var recommend = this._context.Activity.Where(x => x.Title.Contains(activity.SearchContent) && x.FanpageId != null)
                        .Include(x => x.Fanpage)
                        .OrderByDescending(x => x.CreateAt).Take(2);
                    foreach (var lix in recommend)
                    {
                        //var fanpage = await this._context.Fanpage.Where(x => x.FanpageId.Equals(lix.FanpageId)).FirstOrDefaultAsync();
                        li.Add(lix.Fanpage);
                    }
                }
                return li;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
