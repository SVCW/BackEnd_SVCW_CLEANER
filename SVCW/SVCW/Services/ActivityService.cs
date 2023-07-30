using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Activities;
using SVCW.DTOs.Config;
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
                var ad = new adminConfig();
                var config = new ConfigService();
                ad = config.GetAdminConfig();
                decimal donate = 0;
                var user = await this.context.User.Where(x=>x.UserId.Equals(dto.UserId)).Include(x=>x.Fanpage).FirstOrDefaultAsync();
                if (!dto.isFanpageAvtivity)
                {
                    if (user.NumberActivityJoin >= ad.NumberActivityJoinSuccess1)
                    {
                        donate = (decimal)ad.maxTargetDonate1;
                    }
                    if (user.NumberActivityJoin >= ad.NumberActivityJoinSuccess2)
                    {
                        donate = (decimal)ad.maxTargetDonate2;
                    }
                    if (user.NumberActivityJoin >= ad.NumberActivityJoinSuccess3)
                    {
                        donate = (decimal)ad.maxTargetDonate3;
                    }
                    if (dto.TargetDonation > donate)
                    {
                        throw new Exception("số tiền quyên góp tối đa bạn có thể kêu gọi là: " + donate);
                    }
                }
                else if(user.Fanpage != null)
                {
                    donate = (decimal)ad.maxTargetDonate3;
                }
                

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
                activity.Status = "Active";
                activity.RealDonation = 0;
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
                    if(check.RealDonation > 0)
                    {
                        throw new Exception("activity have donate can't remove");
                    }
                    check.Status= "InActive";
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

        public async Task<Activity> deleteAdmin(string id)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.ActivityId.Equals(id)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Status = "InActive";
                    return check;
                }
                else
                {
                    throw new Exception("not found activity");
                }
            }
            catch (Exception ex)
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
                    check.IsFollow = false;
                    this.context.FollowJoinAvtivity.Update(check);
                    await this.context.SaveChangesAsync();
                    var ac = await this.context.Activity.Where(x=>x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                    if (ac != null)
                    {
                        ac.NumberJoin -= 1;
                        this.context.Activity.Update(ac);
                        await this.context.SaveChangesAsync();
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
                var check = await this.context.FollowJoinAvtivity
                    .Where(x => x.UserId.Equals(userId) && x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if(check != null)
                {
                    check.IsFollow = true;
                    this.context.FollowJoinAvtivity.Update(check);
                    await this.context.SaveChangesAsync();
                    return true;
                }
                var follow = new FollowJoinAvtivity();
                follow.UserId = userId;
                follow.ActivityId = activityId;
                follow.IsJoin = false;
                follow.IsFollow= true;
                follow.Datetime = DateTime.Now;

                await this.context.FollowJoinAvtivity.AddAsync(follow);
                await this.context.SaveChangesAsync();
                return true;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getActivityAfterEndDate()
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.EndDate < DateTime.Now)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
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
                        .ThenInclude(x => x.User)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount)
                    .OrderByDescending(x => x.CreateAt)
                    .ToListAsync();
                if(check != null)
                {
                    return check;
                }
                return null;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getActivityBeforeEndDate()
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.EndDate > DateTime.Now && x.StartDate<DateTime.Now)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
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
                        .ThenInclude(x => x.User)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount)
                    .OrderByDescending(x => x.CreateAt)
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

        public async Task<List<Activity>> getActivityBeforeStartDate()
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.StartDate > DateTime.Now)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
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
                        .ThenInclude(x => x.User)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount)
                    .OrderByDescending(x => x.CreateAt)
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

        public async Task<List<Activity>> getActivityFanpage(string fanpageId)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.FanpageId.Equals(fanpageId))
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                        .ThenInclude(x => x.InverseReply.OrderByDescending(x=>x.Datetime))
                            .ThenInclude(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                        .ThenInclude(x=>x.User)
                    .Include(x => x.Process.OrderBy(x=>x.ProcessNo).Where(x => x.Status))
                        .ThenInclude(x => x.Media)
                    .Include(x => x.Donation)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                        .ThenInclude(x=>x.User)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount)
                    .OrderByDescending(x=>x.CreateAt)
                    .ToListAsync();
                if(check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("Fanpage have no activity");
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getActivityUser(string userId)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.UserId.Equals(userId))
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
                    .Include(x => x.BankAccount)
                    .OrderByDescending(x => x.CreateAt)
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("User have no activity");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getAll(int pageSize, int PageLoad)
        {
            try
            {
                var result = new List<Activity>();
                if(PageLoad == 1)
                {
                    var check = this.context.Activity
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
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
                        .ThenInclude(x => x.User)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount)
                    .OrderByDescending(x => x.CreateAt)
                    .Take(pageSize);

                    foreach (var c in check)
                    {
                        result.Add(c);
                    }
                }
                if (PageLoad >1)
                {
                    var check = this.context.Activity
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
                    .Include(x => x.BankAccount)
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
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null))
                        .ThenInclude(x => x.InverseReply.OrderByDescending(x => x.Datetime))
                            .ThenInclude(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Process.OrderBy(x => x.ProcessNo).Where(x=>x.Status))
                        .ThenInclude(x => x.Media)
                    .Include(x => x.Donation)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount)
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
                    .Include(x => x.BankAccount)
                    .Where(x=>x.Title.Contains(title))
                    .OrderByDescending(x => x.CreateAt)
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

        public async Task<List<Activity>> getDataLoginPage()
        {
            try
            {
                var check = await this.context.Activity.OrderByDescending(x => x.NumberLike).Take(3).ToListAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    return null;
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getForUser()
        {
            try
            {
                var check = await this.context.Activity
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
                    .Include(x => x.BankAccount)
                    .Where(x=>x.Status == "Active")
                    .OrderByDescending(x => x.CreateAt)
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
                var check = await this.context.FollowJoinAvtivity
                    .Where(x => x.UserId.Equals(userId) && x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.IsJoin = true;
                    check.IsFollow = true;
                    this.context.FollowJoinAvtivity.Update(check);
                    return await this.context.SaveChangesAsync() > 0;
                }
                var follow = new FollowJoinAvtivity();
                follow.UserId = userId;
                follow.ActivityId = activityId;
                follow.IsJoin = true;
                follow.IsFollow = true;
                follow.Datetime = DateTime.Now;

                await this.context.FollowJoinAvtivity.AddAsync(follow);

                var check2 = await this.context.Activity.Where(x=>x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                check2.NumberJoin += 1;
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
                this.context.FollowJoinAvtivity.Update(check);

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
                this.context.Activity.Update(check);
                if(await this.context.SaveChangesAsync() >0)
                {
                    return check;
                }
                return null;
               
            }catch (Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
