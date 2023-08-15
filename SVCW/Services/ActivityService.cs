using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using SVCW.DTOs.Activities;
using SVCW.DTOs.Config;
using SVCW.Interfaces;
using SVCW.Models;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

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
                activity.Location= dto.Location ?? "";
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

                    this.context.Activity.Update(check);

                    await this.context.SaveChangesAsync();
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

                    this.context.Activity.Update(check);

                    await this.context.SaveChangesAsync();
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
                    var ac1 = await this.context.Process.Where(x => x.ActivityId.Equals(activityId)).ToListAsync();
                    if (ac1 != null)
                    {
                        foreach (var x in ac1)
                        {
                            if (x.ProcessTypeId.Equals("pt002"))
                            {
                                if (DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate)
                                {
                                    if (x.RealParticipant <= x.TargetParticipant)
                                    {
                                        x.RealParticipant += 1;
                                        this.context.Process.Update(x);
                                        await this.context.SaveChangesAsync();
                                    }

                                }
                            }
                        }
                    }
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
                var check = await this.context.Activity.Where(x => x.EndDate < DateTime.Now && x.Status.Equals("Active"))
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
                var check = await this.context.Activity.Where(x => x.EndDate > DateTime.Now && x.StartDate<DateTime.Now && x.Status.Equals("Active"))
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
                var check = await this.context.Activity.Where(x => x.StartDate > DateTime.Now && x.Status.Equals("Active"))
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

        public async Task<List<Activity>> getActivityBeforeStartDateUser(string userId)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.StartDate > DateTime.Now && x.UserId.Equals(userId) && x.Status.Equals("Active"))
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
                var check = await this.context.Activity.Where(x => x.FanpageId.Equals(fanpageId) && x.Status.Equals("Active"))
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
                var check = await this.context.Activity.Where(x => x.UserId.Equals(userId) && x.Status.Equals("Active"))
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
                    .Where(x=>x.ActivityId.Equals(id))
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null))
                        .ThenInclude(x => x.InverseReply.OrderByDescending(x => x.Datetime)).ThenInclude(x=>x.User)
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
                    .Where(x => x.Title.Contains(title) && x.Status.Equals("Active"))
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
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<SearchResultDTO> search(string searchContent)
        {
            try
            {
                var result = new SearchResultDTO();
                var check = await this.context.Activity
                    .Where(x => x.Title.Contains(searchContent) && x.Status.Equals("Active"))
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
                result.activities = check;

                var check2 = await this.context.Fanpage.Where(x => x.FanpageName.Contains(searchContent) && x.Status.Equals("Active"))
                    .Include(x => x.Activity.OrderByDescending(x => x.CreateAt))
                        .ThenInclude(x => x.Comment.Where(b => b.ReplyId == null).OrderByDescending(x => x.Datetime))
                            .ThenInclude(x => x.InverseReply)
                                .ThenInclude(x => x.User)
                    .Include(x => x.Activity.OrderByDescending(x => x.CreateAt))
                        .ThenInclude(x => x.Comment.Where(b => b.ReplyId == null).OrderByDescending(x => x.Datetime))
                            .ThenInclude(x => x.User)
                    .Include(x => x.FollowFanpage)
                    .Include(x => x.Activity.OrderByDescending(x => x.CreateAt))
                        .ThenInclude(x => x.User)
                    //.Include(x => x.FanpageNavigation)
                    .ToListAsync();

                result.fanpages = check2;

                var user = await this.context.User
                   .Include(u => u.Activity.OrderByDescending(x => x.CreateAt).Where(x => x.Status.Equals("Active")))
                   .Include(u => u.Fanpage)                                            // Include the related fanpage
                   .Include(u => u.Donation)
                   .Include(u => u.FollowJoinAvtivity)
                   .Include(u => u.AchivementUser)
                   .Include(u => u.Report)
                   .Include(u => u.BankAccount)
                   .Include(u => u.Like)
                   .Include(u => u.VoteUserVote)
                   .Include(u => u.AchivementUser)
                       .ThenInclude(u => u.Achivement)
                   .Include(u => u.FollowFanpage)
                       .ThenInclude(u => u.Fanpage)
                   .Where(u => u.FullName.Contains(searchContent) || u.Username.Contains(searchContent))
                   .ToListAsync();
                result.users = user;
                if (result != null)
                {
                    return result;
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
                var check = await this.context.Activity
                    .Include(x=>x.Media)
                    .Include(x=>x.User)
                    .Include(x=>x.Fanpage)
                    .OrderByDescending(x => x.NumberLike).Where(x=>x.Status.Equals("Active")).Take(3).ToListAsync();
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

        public async Task<List<Activity>> getForUser(int pageSize, int PageLoad)
        {
            try
            {
                var result = new List<Activity>();
                if (PageLoad == 1)
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
                    .Where(x=> x.Status.Equals("Active"))
                    .Take(pageSize);

                    foreach (var c in check)
                    {
                        result.Add(c);
                    }
                }
                if (PageLoad > 1)
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
                    .Where(x => x.Status.Equals("Active"))
                    .Take(PageLoad * pageSize - pageSize);
                    foreach (var x in check)
                    {
                        result.Add(x);
                    }
                }
                if (result != null)
                {
                    return result;
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
                var ac = await this.context.Process.Where(x => x.ActivityId.Equals(activityId)).ToListAsync();
                if (ac != null)
                {
                    foreach(var x in ac)
                    {
                        if (x.ProcessTypeId.Equals("pt002"))
                        {
                            if(DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate)
                            {
                                if(x.RealParticipant >= x.TargetParticipant)
                                {
                                    throw new Exception("đã đủ người tham gia hoạt động, bạn hãy chờ hoạt động lần sau (nếu có)");
                                }

                            }
                            else
                            {
                                throw new Exception("chưa tới hạn tham gia hoặc đã quá hạn");
                            }
                        }
                    }
                }
                var check = await this.context.FollowJoinAvtivity
                    .Where(x => x.UserId.Equals(userId) && x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.IsJoin = true;
                    check.IsFollow = true;
                    this.context.FollowJoinAvtivity.Update(check);
                    await this.context.SaveChangesAsync();
                    var c2 = await this.context.Activity.Where(x => x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                    c2.NumberJoin += 1;
                    this.context.Activity.Update(c2);
                    await this.context.SaveChangesAsync();

                    var ac1 = await this.context.Process.Where(x => x.ActivityId.Equals(activityId)).ToListAsync();
                    if (ac1 != null)
                    {
                        foreach (var x in ac1)
                        {
                            if (x.ProcessTypeId.Equals("pt002"))
                            {
                                if (DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate)
                                {
                                    if (x.RealParticipant <= x.TargetParticipant)
                                    {
                                        x.RealParticipant += 1;
                                        this.context.Process.Update(x);
                                        await this.context.SaveChangesAsync();
                                    }

                                }
                            }   
                        }
                    }

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
                this.context.Activity.Update(check2);

                var acc = await this.context.Process.Where(x => x.ActivityId.Equals(activityId)).ToListAsync();
                if (acc != null)
                {
                    foreach (var x in acc)
                    {
                        if (x.ProcessTypeId.Equals("pt002"))
                        {
                            if (DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate)
                            {
                                if (x.RealParticipant <= x.TargetParticipant)
                                {
                                    x.RealParticipant += 1;
                                    this.context.Process.Update(x);
                                    await this.context.SaveChangesAsync();
                                }

                            }
                        }
                    }
                }
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
