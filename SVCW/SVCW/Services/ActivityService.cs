using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Activities;
using SVCW.Interfaces;
using SVCW.Models;
using System.Collections.Immutable;
using System.Linq;

namespace SVCW.Services
{
    public class ActivityService : IActivity
    {
        protected readonly SVCWContext context;
        public ActivityService(SVCWContext context)
        {
            this.context = context;
        }
        public async Task<Activity> createActivity(ActivityCreateDTO dto)
        {
            try
            {
                var activity = new Activity();
                activity.ActivityId = "ACT" + Guid.NewGuid().ToString().Substring(0,7);
                activity.Title= dto.Title;
                activity.Description= dto.Description;
                activity.CreateAt= DateTime.Now;
                activity.StartDate = dto.StartDate;
                activity.EndDate = dto.EndDate;
                activity.Location= dto.Location;
                activity.NumberJoin = 0;
                activity.NumberLike= 0;
                activity.ShareLink = "chưa làm dc";
                activity.TargetDonation = dto.TargetDonation;
                activity.UserId= dto.UserId;
                activity.Status = "1";
                if (dto.isFanpageAvtivity)
                {
                    activity.FanpageId = dto.UserId;
                }

                await this.context.Activity.AddAsync(activity);
                
                if(await this.context.SaveChangesAsync() > 0)
                {
                    foreach (var media in dto.media)
                    {
                        var media2 = new Media();
                        media2.MediaId = "MED" + Guid.NewGuid().ToString().Substring(0, 7);
                        media2.Type = media.Type;
                        media2.LinkMedia = media.LinkMedia;
                        media2.ActivityId = activity.ActivityId;
                        await this.context.Media.AddAsync(media2);
                        await this.context.SaveChangesAsync();
                        media2 = new Media();
                    }
                    return activity;
                }
                return null;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Activity> delete(string id)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.ActivityId.Equals(id)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Status= "0";
                    return check;
                }
                else
                {
                    throw new Exception("not found activity");
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> disJoinActivity(string activityId, string userId)
        {
            try
            {
                var check = await this.context.FollowJoinAvtivity.Where(x=>x.UserId.Equals(userId) && x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if(check != null)
                {
                    check.IsJoin = false;
                    var ac = await this.context.Activity.Where(x=>x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                    if (ac != null)
                    {
                        ac.NumberJoin -= 1;
                        return true;
                    }
                }
                return false;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> followActivity(string activityId, string userId)
        {
            try
            {
                var follow = new FollowJoinAvtivity();
                follow.UserId = userId;
                follow.ActivityId = activityId;
                follow.IsJoin = false;
                follow.IsFollow= true;
                follow.Datetime = DateTime.Now;

                await this.context.FollowJoinAvtivity.AddAsync(follow);
                return await this.context.SaveChangesAsync() > 0;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public List<Activity> getAll(int pageSize, int PageLoad)
        {
            try
            {
                var result = new List<Activity>();
                if(PageLoad == 1)
                {
                     var check = this.context.Activity
                    .Include(x => x.Comment)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                    .Include(x => x.Process)
                    //.Include(x => x.Donation)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                    .Include(x => x.Media)
                    //.Include(x => x.BankAccount)
                    .OrderByDescending(x => x.CreateAt)
                    .Take(pageSize);
                    foreach(var x in check)
                    {
                        result.Add(x);
                    }
                }
                if(PageLoad >1)
                {
                    var check = this.context.Activity
                    .Include(x => x.Comment)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                    .Include(x => x.Process)
                    //.Include(x => x.Donation)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                    .Include(x => x.Media)
                    //.Include(x => x.BankAccount)
                    .OrderByDescending(x => x.CreateAt)
                    .Take(PageLoad*pageSize - pageSize);
                    foreach (var x in check)
                    {
                        result.Add(x);
                    }
                }
                if(result != null)
                {
                    return result;
                }
                return null;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Activity> getById(string id)
        {
            try
            {
                var check = await this.context.Activity
                    .Include(x => x.Comment)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                    .Include(x => x.Process)
                    .Include(x => x.Donation)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount)
                    .Where(x=>x.ActivityId== id)
                    .FirstOrDefaultAsync();
                if (check != null)
                {
                    return check;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getByTitle(string title)
        {
            try
            {
                var check = await this.context.Activity
                    .Include(x => x.Comment)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                    .Include(x => x.Process)
                    .Include(x => x.Donation)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount)
                    .Where(x=>x.Title.Contains(title))
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getForUser()
        {
            try
            {
                var check = await this.context.Activity
                    .Include(x => x.Comment)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                    .Include(x => x.Process)
                    .Include(x => x.Donation)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount)
                    .Where(x=>x.Status =="1")
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> joinActivity(string activityId, string userId)
        {
            try
            {
                var follow = new FollowJoinAvtivity();
                follow.UserId = userId;
                follow.ActivityId = activityId;
                follow.IsJoin = true;
                follow.IsFollow = true;
                follow.Datetime = DateTime.Now;

                await this.context.FollowJoinAvtivity.AddAsync(follow);

                var check = await this.context.Activity.Where(x=>x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                check.NumberJoin += 1;
                return await this.context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> unFollowActivity(string activityId, string userId)
        {
            try
            {
                var check = await this.context.FollowJoinAvtivity.Where(x => x.UserId.Equals(userId) && x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.IsFollow = false;
                }
                return await this.context.SaveChangesAsync() >0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Activity> updateActivity(ActivityUpdateDTO dto)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.ActivityId == dto.ActivityId).FirstOrDefaultAsync();
                check.Title = dto.Title;
                check.Description = dto.Description;
                check.StartDate = dto.StartDate;
                check.EndDate = dto.EndDate;
                check.StartDate = dto.StartDate;
                check.Location = dto.Location;
                check.TargetDonation = dto.TargetDonation;
                if(await this.context.SaveChangesAsync() >0)
                {
                    return check;
                }
                return null;
               
            }catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
