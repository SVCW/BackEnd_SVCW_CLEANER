using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Ocsp;
using SVCW.DTOs.Activities;
using SVCW.DTOs.Config;
using SVCW.DTOs.Email;
using SVCW.Interfaces;
using SVCW.Models;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Xml.Linq;

namespace SVCW.Services
{
    public class ActivityService : IActivity
    {
        protected readonly SVCWContext context;

        private readonly IEmail _service;
        public ActivityService(SVCWContext context, IEmail service)
        {
            this.context = context;
            _service = service;
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
                        throw new Exception("Số tiền quyên góp tối đa bạn có thể kêu gọi là: " + donate);
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
                activity.StartDate = (DateTime)dto.StartDate;
                activity.EndDate = (DateTime)dto.EndDate;
                activity.Location= dto.Location ?? "online";
                activity.NumberJoin = 0;
                activity.NumberLike= 0;
                activity.ShareLink = "https://wscv-fe-wscv-fe.vercel.app/detailactivity/" + activity.ActivityId;
                activity.TargetDonation = 0;
                activity.UserId= dto.UserId;
                activity.Status = "Pending";
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
                var activity = await this.context.Activity.Where(x => x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if (activity.Status.Equals("Pending"))
                {
                    throw new Exception("Chiến dịch chưa được duyệt");
                }
                if (activity.Status.Equals("Reject"))
                {
                    throw new Exception("Chiến dịch bị từ chối");
                }
                if (activity.Status.Equals("Quit"))
                {
                    throw new Exception("Chiến dịch đã bị chủ sở hữu hủy sớm nên bạn không thể thực hiện hành động này");
                }
                var check = await this.context.FollowJoinAvtivity.Where(x=>x.UserId.Equals(userId) && x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if(check != null)
                {
                    check.IsJoin = "unJoin";
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
                                        x.RealParticipant -= 1;
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
                var activity = await this.context.Activity.Where(x => x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if (activity.Status.Equals("Pending"))
                {
                    throw new Exception("Chiến dịch chưa được duyệt");
                }
                if (activity.Status.Equals("Reject"))
                {
                    throw new Exception("Chiến dịch bị từ chối");
                }
                if (activity.Status.Equals("Quit"))
                {
                    throw new Exception("Chiến dịch đã bị chủ sở hữu hủy sớm nên bạn không thể thực hiện hành động này");
                }
                var check = await this.context.FollowJoinAvtivity
                    .Where(x => x.UserId.Equals(userId) && x.ActivityId.Equals(activityId))
                    .FirstOrDefaultAsync();
                if(check != null)
                {
                    check.IsFollow = true;
                    this.context.FollowJoinAvtivity.Update(check);
                    await this.context.SaveChangesAsync();
                    return true;
                }
                var process = await this.context.Process.Where(x => x.ActivityId.Equals(activityId)).ToListAsync();
                string tmpProcess = null;
                if (process != null && process.Count >0)
                {
                    foreach(var x in process)
                    {
                            tmpProcess = x.ProcessId;
                            break;
                    }
                }
                var follow = new FollowJoinAvtivity();
                follow.UserId = userId;
                follow.ActivityId = activityId;
                follow.IsJoin = "unJoin";
                follow.IsFollow= true;
                follow.Datetime = DateTime.Now;
                if(tmpProcess != null)
                {
                    follow.ProcessId = tmpProcess;
                }
                else
                {
                    throw new Exception("có lỗi xảy ra trong quá trình theo dõi chiến dịch do chiến dịch không có hoạt động nào");
                }

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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
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
                var check = await this.context.Activity.Where(x => x.EndDate < DateTime.Now && x.UserId.Equals(userId) && x.Status.Equals("Active"))
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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
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
                    var check = await this.context.Activity
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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
                    .OrderByDescending(x => x.CreateAt)
                    .Take(pageSize).ToListAsync();

                    foreach (var c in check)
                    {
                        result.Add(c);
                    }
                }
                if (PageLoad >1)
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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
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

        public async Task<List<Activity>> getByTitle(SearchDTO title)
        {
            try
            {
                var check = await this.context.Activity
                    .Where(x => x.Title.Contains(title.search) && x.Status.Equals("Active"))
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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
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

        public async Task<SearchResultDTO> search(SearchDTO searchContent )
        {
            try
            {
                var result = new SearchResultDTO();
                var check = await this.context.Activity
                    .Where(x => x.Title.Contains(searchContent.search) && x.Status.Equals("Active"))
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
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
                    .OrderByDescending(x => x.CreateAt)
                    .ToListAsync();
                result.activities = check;

                var check2 = await this.context.Fanpage.Where(x => x.FanpageName.Contains(searchContent.search) && x.Status.Equals("Active"))
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
                   .Include(u => u.ReportUser)
                   .Include(u => u.BankAccount)
                   .Include(u => u.Like)
                   .Include(u => u.VoteUserVote)
                   .Include(u => u.AchivementUser)
                       .ThenInclude(u => u.Achivement)
                   .Include(u => u.FollowFanpage)
                       .ThenInclude(u => u.Fanpage)
                   .Where(u => (u.FullName.Contains(searchContent.search) || u.Username.Contains(searchContent.search)) && u.UserId.Contains("USR") )
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
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                        .ThenInclude(x => x.User)
                    //.Include(x => x.Process.OrderBy(x => x.ProcessNo).Where(x => x.Status))
                    //    .ThenInclude(x => x.Media)
                    .Include(x => x.Donation)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Media)
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
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null).Take(3))
                        .ThenInclude(x => x.InverseReply.OrderByDescending(x => x.Datetime))
                            .ThenInclude(x => x.User)
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                        .ThenInclude(x => x.User)
                    //.Include(x => x.Process.OrderBy(x => x.ProcessNo).Where(x => x.Status))
                    //    .ThenInclude(x => x.Media)
                    .Include(x => x.Donation)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Media)
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
                var activity = await this.context.Activity.Where(x => x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if (activity.Status.Equals("Pending"))
                {
                    throw new Exception("Chiến dịch chưa được duyệt");
                }
                if (activity.Status.Equals("Reject"))
                {
                    throw new Exception("Chiến dịch bị từ chối");
                }
                if (activity.Status.Equals("Quit"))
                {
                    throw new Exception("Chiến dịch đã bị chủ sở hữu hủy sớm nên bạn không thể thực hiện hành động này");
                }
                string tmpProcess = null;
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
                                    throw new Exception("Đã đủ người tham gia hoạt động, bạn hãy chờ hoạt động lần sau (nếu có)");
                                }
                                tmpProcess = x.ProcessId;
                            }
                            else
                            {
                                throw new Exception("Chưa tới hạn tham gia hoặc đã quá hạn");
                            }
                        }
                    }
                }
                var check = await this.context.FollowJoinAvtivity
                    .Where(x => x.UserId.Equals(userId) && x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.IsJoin = "Join";
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

                    return true;
                }
                var follow = new FollowJoinAvtivity();
                follow.UserId = userId;
                follow.ActivityId = activityId;
                follow.IsJoin = "Join";
                follow.IsFollow = true;
                follow.ProcessId = tmpProcess;
                follow.Datetime = DateTime.Now;

                await this.context.FollowJoinAvtivity.AddAsync(follow);
                await this.context.SaveChangesAsync();

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
                return true;
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
                var activity = await this.context.Activity.Where(x => x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                if (activity.Status.Equals("Pending"))
                {
                    throw new Exception("Chiến dịch chưa được duyệt");
                }
                if (activity.Status.Equals("Reject"))
                {
                    throw new Exception("Chiến dịch bị từ chối");
                }
                if (activity.Status.Equals("Quit"))
                {
                    throw new Exception("Chiến dịch đã bị chủ sở hữu hủy sớm nên bạn không thể thực hiện hành động này");
                }
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
                check.StartDate = (DateTime)dto.StartDate;
                check.EndDate = (DateTime)dto.EndDate;
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

        public async Task<Activity> activePending(string id)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.ActivityId.Equals(id)).FirstOrDefaultAsync();
                if(check != null)
                {
                    check.Status = "Active";
                    this.context.Activity.Update(check);
                    if(await this.context.SaveChangesAsync() > 0)
                    {
                        return check;
                    }
                    else
                    {
                        throw new Exception("fail active pending");
                    }
                }
                else
                {
                    throw new Exception("not found");
                }
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Activity> reActive(string id)
        {

            try
            {
                var check = await this.context.Activity.Where(x => x.ActivityId.Equals(id))
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
                    .FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Status = "Active";
                    this.context.Activity.Update(check);
                    if (await this.context.SaveChangesAsync() > 0)
                    {
                        var rej = await this.context.RejectActivity.Where(x => x.ActivityId.Equals(check.ActivityId)).ToListAsync();
                        if (rej != null)
                        {

                            foreach(var x in rej)
                            {
                                x.Status = false;
                                this.context.RejectActivity.Update(x);
                                await this.context.SaveChangesAsync();
                            }
                        }
                        var quit = await this.context.QuitActivity.Where(x => x.ActivityId.Equals(check.ActivityId)).ToListAsync();
                        if (quit != null)
                        {
                            foreach(var x in quit)
                            {
                                x.Status = false;
                                this.context.QuitActivity.Update(x);
                                await this.context.SaveChangesAsync();
                            }
                        }
                        return check;
                    }
                    else
                    {
                        throw new Exception("fail active pending");
                    }
                }
                else
                {
                    throw new Exception("not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getActivityPending()
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.Status.Equals("Pending"))
                    .Include(x => x.Media)
                    .Include(x => x.Process)
                        .ThenInclude(x => x.Media)
                    .Include(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
                    .OrderByDescending(x=>x.CreateAt)
                    .ToListAsync();
                if(check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("not found");
                }
            }catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Activity> rejectActivity(RejectActivityDTO dto)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.ActivityId.Equals(dto.activityId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Status = "Reject";
                    this.context.Activity.Update(check);
                    if (await this.context.SaveChangesAsync() > 0)
                    {
                        var rej = new RejectActivity();
                        rej.ActivityId = dto.activityId;
                        rej.Reason = dto.reasonReject;
                        rej.Datetime = DateTime.Now;
                        rej.Status = true;
                        rej.RejectId = "RJA" + Guid.NewGuid().ToString().Substring(0, 7);

                        await this.context.RejectActivity.AddAsync(rej);
                        await this.context.SaveChangesAsync();

                        return check;
                    }
                    else
                    {
                        throw new Exception("fail reject");
                    }
                }
                else
                {
                    throw new Exception("not found");
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getActivityReject(string userId)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.Status.Equals("Reject") && x.UserId.Equals(userId))
                    .Include(x => x.Media)
                    .Include(x => x.Process)
                        .ThenInclude(x => x.Media)
                    .Include(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
                    .OrderByDescending(x => x.CreateAt)
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("not found");
                }
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getActivityRejectAdmin()
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.Status.Equals("Reject"))
                    .Include(x => x.Media)
                    .Include(x => x.Process)
                        .ThenInclude(x => x.Media)
                    .Include(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
                    .OrderByDescending(x => x.CreateAt)
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> checkIn(string activityId, string userId)
        {
            try
            {
                var check = await this.context.Process.Where(x => x.ActivityId.Equals(activityId) && x.ProcessTypeId.Equals("pt003"))
                    .ToListAsync();
                if(check != null && check.Count>0)
                {
                    foreach(var x in check)
                    {
                        if(x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now)
                        {
                            var participant = await this.context.FollowJoinAvtivity.Where(a => a.ActivityId.Equals(activityId) && a.UserId.Equals(userId) && a.IsJoin.Equals("Join")).ToListAsync();
                            var participant1 = await this.context.FollowJoinAvtivity.Where(a => a.ActivityId.Equals(activityId) && a.UserId.Equals(userId) && a.IsJoin.Equals("success")).ToListAsync();
                            if (participant1 != null && participant1.Count > 0)
                            {
                                throw new Exception("Tình nguyện viên đã điểm danh");
                            }
                            if (participant != null && participant.Count > 0)
                            {
                                foreach(var u in participant)
                                {
                                    u.IsJoin = "success";
                                    this.context.FollowJoinAvtivity.Update(u);
                                    await this.context.SaveChangesAsync();
                                }
                                var user = await this.context.User.Where(v => v.UserId.Equals(userId)).FirstOrDefaultAsync();
                                user.NumberActivityJoin += 1;
                                this.context.User.Update(user);
                                await this.context.SaveChangesAsync();

                                if(user.NumberActivityJoin == 1)
                                {
                                    var acus = new AchivementUser();
                                    acus.AchivementId = "AId7658264";
                                    acus.UserId = user.UserId;
                                    acus.EndDate = DateTime.MaxValue;

                                    await this.context.AchivementUser.AddAsync(acus);

                                    await this.context.SaveChangesAsync();
                                    var noti = new Notification();
                                    noti.UserId = user.UserId;
                                    noti.Datetime = DateTime.Now;
                                    noti.Status = true;
                                    noti.Title = "Trao tham gia chiến dịch đầu tiên";
                                    noti.NotificationContent = "Bạn đã nhận được huy hiệu sau khi tham gia thành công chiến dịch đầu tiên";
                                    noti.ActivityId = activityId;
                                    noti.NotificationId = "Noti" + Guid.NewGuid().ToString().Substring(0, 6);
                                    await this.context.Notification.AddAsync(noti);
                                    await this.context.SaveChangesAsync();
                                }
                                if (user.NumberActivityJoin == 5)
                                {
                                    var acus = new AchivementUser();
                                    acus.AchivementId = "AIdf8af790";
                                    acus.UserId = user.UserId;
                                    acus.EndDate = DateTime.MaxValue;

                                    await this.context.AchivementUser.AddAsync(acus);

                                    await this.context.SaveChangesAsync();
                                    var noti = new Notification();
                                    noti.UserId = user.UserId;
                                    noti.Datetime = DateTime.Now;
                                    noti.Status = true;
                                    noti.Title = "Trao huy tham gia đủ 5 chiến dịch";
                                    noti.NotificationContent = "Bạn đã nhận được huy hiệu sau khi tham gia thành công chiến dịch thứ 5, bây giờ bạn có thể tạo chiến dịch cho bản thân mình";
                                    noti.ActivityId = activityId;
                                    noti.NotificationId = "Noti" + Guid.NewGuid().ToString().Substring(0, 6);
                                    await this.context.Notification.AddAsync(noti);
                                    await this.context.SaveChangesAsync();
                                }
                                if (user.NumberActivityJoin == 10)
                                {
                                    var acus = new AchivementUser();
                                    acus.AchivementId = "AId998a9a6";
                                    acus.UserId = user.UserId;
                                    acus.EndDate = DateTime.MaxValue;

                                    await this.context.AchivementUser.AddAsync(acus);

                                    await this.context.SaveChangesAsync();
                                    var noti = new Notification();
                                    noti.UserId = user.UserId;
                                    noti.Datetime = DateTime.Now;
                                    noti.Status = true;
                                    noti.Title = "Trao huy hiệu tham gia đủ 10 chiến dịch";
                                    noti.NotificationContent = "Bạn đã nhận được huy hiệu sau khi tham gia thành công 10 chiến dịch trên hệ thống SVCW";
                                    noti.ActivityId = activityId;
                                    noti.NotificationId = "Noti" + Guid.NewGuid().ToString().Substring(0, 6);
                                    await this.context.Notification.AddAsync(noti);
                                    await this.context.SaveChangesAsync();
                                }
                                return true;
                            }
                            else
                            {
                                throw new Exception("Tình nguyện viên không đăng kí tham gia chiến dịch");
                            }
                        }
                        else
                        {
                            throw new Exception("Hoạt động này đang không diễn ra hoặc đã kết thúc");
                        }
                    }
                }
                else
                {
                    throw new Exception("Chiến dịch không có hoạt động cần điểm danh");
                }
                return false;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Activity> checkQR(string activityId)
        {
            try
            {
                var check = await this.context.Process.Where(x => x.ActivityId.Equals(activityId) && x.ProcessTypeId.Equals("pt003") && x.StartDate <= DateTime.Now && x.EndDate >= DateTime.Now)
                    .ToListAsync();
                if(check != null)
                {
                    var activity = await this.context.Activity.Where(x => x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                    if(activity != null)
                    {
                        return activity;
                    }
                    else
                    {
                        throw new Exception("not found");
                    }
                }
                else
                {
                    throw new Exception("Chiến dịch không có hoạt động cần điểm danh");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Activity> quitActivity(QuitActivityDTO dto)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.ActivityId.Equals(dto.activityId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Status = "Quit";
                    this.context.Activity.Update(check);
                    if (await this.context.SaveChangesAsync() > 0)
                    {
                        var rej = new QuitActivity();
                        rej.ActivityId = dto.activityId;
                        rej.Reason = dto.reasonQuit;
                        rej.CreateAt = DateTime.Now;
                        rej.Status = true;
                        rej.QuitActivityId = "QIT" + Guid.NewGuid().ToString().Substring(0, 7);

                        await this.context.QuitActivity.AddAsync(rej);
                        await this.context.SaveChangesAsync();

                        var flj = await this.context.FollowJoinAvtivity.Where(x=>x.ActivityId.Equals(dto.activityId) && x.IsFollow == true).ToListAsync();
                        var dnt = await this.context.Donation.Where(x=>x.ActivityId.Equals(dto.activityId)).ToListAsync();
                        if(flj.Count > 0 || dnt.Count > 0)
                        {
                            foreach(var f in flj)
                            {
                                var email = new SendEmailReqDTO();
                                foreach (var d in dnt)
                                {
                                    email = new SendEmailReqDTO();
                                    email.sendTo = d.Email;
                                    email.subject = "Chiến dịch "+check.Title+" đã được chủ sở hữu dừng hoạt động trên hệ thống SVCW";
                                    email.body = "<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <title>Thông báo về chiến dịch bạn ủng hộ đã ngừng hoạt động </title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n        }\r\n\r\n        .container {\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            border: 1px solid #ccc;\r\n            border-radius: 5px;\r\n        }\r\n\r\n        .header {\r\n            background-color: #FFC107;\r\n            color: #fff;\r\n            text-align: center;\r\n            padding: 10px;\r\n        }\r\n\r\n        .content {\r\n            padding: 20px;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"container\">\r\n    <div class=\"header\">\r\n        <h1>Chiến dịch "+check.Title+" đã ngừng hoạt động</h1>\r\n    </div>\r\n    <div class=\"content\">\r\n        <p>Xin chào " + d.Name + ",</p>\r\n        <p>Chúng tôi hy vọng bạn đang có một ngày tốt lành.</p>\r\n        <p>Chúng tôi xin thông báo rằng chiến dịch "+check.Title+" đã ngừng hoạt động với lý do: "+rej.Reason+" </p>\r\n        <p>Hệ thống SVCW sẽ hoàn trả lại số tiền mà bạn đã quyên góp cho chiến dịch này trong khoảng thời gian sớm nhất!</p>\r\n        <p>Nếu bạn cần thêm thông tin hoặc có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua địa chỉ email svcw.company@gmail.com</p>\r\n        <p>Xin cảm ơn bạn và chúc bạn có một ngày tốt lành.</p>\r\n        <p>Trân trọng,<br>Hệ thống SVCW</p>\r\n    </div>\r\n</div>\r\n</body>\r\n</html>\r\n";
                                    _service.sendEmail(email);
                                }
                                var user = await this.context.User.Where(x => x.UserId.Equals(f.UserId)).FirstOrDefaultAsync();
                                email = new SendEmailReqDTO();
                                email.sendTo = user.Email;
                                email.subject = "Chiến dịch " + check.Title + " đã được chủ sở hữu dừng hoạt động trên hệ thống SVCW";
                                email.body = "<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n    <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\r\n    <title>Thông báo về chiến dịch bạn tham gia, theo dõi đã ngừng hoạt động </title>\r\n    <style>\r\n        body {\r\n            font-family: Arial, sans-serif;\r\n        }\r\n\r\n        .container {\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n            padding: 20px;\r\n            border: 1px solid #ccc;\r\n            border-radius: 5px;\r\n        }\r\n\r\n        .header {\r\n            background-color: #FFC107;\r\n            color: #fff;\r\n            text-align: center;\r\n            padding: 10px;\r\n        }\r\n\r\n        .content {\r\n            padding: 20px;\r\n        }\r\n    </style>\r\n</head>\r\n<body>\r\n<div class=\"container\">\r\n    <div class=\"header\">\r\n        <h1>Chiến dịch " + check.Title + " đã ngừng hoạt động</h1>\r\n    </div>\r\n    <div class=\"content\">\r\n        <p>Xin chào " + user.Email + ",</p>\r\n        <p>Chúng tôi hy vọng bạn đang có một ngày tốt lành.</p>\r\n        <p>Chúng tôi xin thông báo rằng chiến dịch " + check.Title + " đã ngừng hoạt động với lý do: " + rej.Reason + " </p>\r\n        <p></p>\r\n        <p>Cảm ơn bạn đã quan tâm, theo dõi tới chiến dịch. Nếu bạn cần thêm thông tin hoặc có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi qua địa chỉ email svcw.company@gmail.com</p>\r\n        <p>Xin cảm ơn bạn và chúc bạn có một ngày tốt lành.</p>\r\n        <p>Trân trọng,<br>Hệ thống SVCW</p>\r\n    </div>\r\n</div>\r\n</body>\r\n</html>\r\n";
                                _service.sendEmail(email);
                            }
                        }
                        return check;
                    }
                    else
                    {
                        throw new Exception("fail quit");
                    }
                }
                else
                {
                    throw new Exception("not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getActivityQuit(string userId)
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.Status.Equals("Quit") && x.UserId.Equals(userId))
                    .Include(x => x.Media)
                    .Include(x => x.Process)
                        .ThenInclude(x => x.Media)
                    .Include(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.QuitActivity)
                    .Include(x => x.RejectActivity)
                    .OrderByDescending(x => x.CreateAt)
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Activity>> getActivityQuitAdmin()
        {
            try
            {
                var check = await this.context.Activity.Where(x => x.Status.Equals("Quit"))
                    .Include(x => x.Media)
                    .Include(x => x.Process)
                        .ThenInclude(x => x.Media)
                    .Include(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x=>x.QuitActivity)
                    .OrderByDescending(x => x.CreateAt)
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("not found");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
