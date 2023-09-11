using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.ActivityResults;
using SVCW.Interfaces;
using SVCW.Models;
using System.Diagnostics;

namespace SVCW.Services
{
    public class ActivityResultService : IActivityResult
    {
        private readonly SVCWContext _context;
        public ActivityResultService(SVCWContext context)
        {
            _context = context;
        }
        public async Task<ActivityResult> create(ActivityResultCreateDTO dto)
        {
            try
            {
                var activityResult = new ActivityResult();
                activityResult.ActivityId= dto.ActivityId;
                activityResult.ResultId = "ACRT" +Guid.NewGuid().ToString().Substring(0,6);
                activityResult.Title = dto.Title;
                activityResult.Desciption = dto.Desciption;
                activityResult.Datetime = DateTime.Now;
                activityResult.TotalAmount = dto.TotalAmount ?? 0;
                activityResult.ResultDocument = dto.ResultDocument ?? null;
                
                await this._context.ActivityResult.AddAsync(activityResult);
                if(await this._context.SaveChangesAsync() >0)
                {
                    foreach (var media in dto.media)
                    {
                        var media2 = new Media();
                        media2.MediaId = "MED" + Guid.NewGuid().ToString().Substring(0, 7);
                        media2.Type = media.Type;
                        media2.LinkMedia = media.LinkMedia;
                        media2.ActivityResultId = activityResult.ResultId;
                        await this._context.Media.AddAsync(media2);
                        await this._context.SaveChangesAsync();
                        media2 = new Media();
                    }
                    return activityResult;
                }
                return new ActivityResult();
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ActivityResult>> getAll()
        {
            try
            {
                var check = await this._context.ActivityResult
                    .Include(x => x.Media)
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.FollowJoinAvtivity.Where(x => x.IsJoin.Equals("success")))
                    .ToListAsync();
                return check;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<ActivityResult>> getForActivity(string activityId)
        {
            try
            {
                var check = await this._context.ActivityResult
                    .Where(x=>x.ActivityId.Equals(activityId))
                    .Include(x=>x.Media)
                    .Include(x=>x.Activity)
                        .ThenInclude(x=>x.Process)
                            .ThenInclude(x=>x.FollowJoinAvtivity.Where(x=>x.IsJoin.Equals("success")))
                                .ThenInclude(x=>x.User)
                    .Include(x=>x.Activity)
                        .ThenInclude(x=>x.Donation)
                            .ThenInclude(x=>x.User)
                    .ToListAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<ActivityResultDTO> getForActivityv2(string activityId)
        {
            try
            {
                var result = new ActivityResultDTO();
                var check = await this._context.ActivityResult
                    .Where(x => x.ActivityId.Equals(activityId))
                    .Include(x => x.Media)
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.Process)
                            .ThenInclude(x => x.FollowJoinAvtivity.Where(x => x.IsJoin.Equals("success")))
                                .ThenInclude(x => x.User)
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.Donation)
                            .ThenInclude(x => x.User)
                    .ToListAsync();

                result.result = check;

                var activity = await this._context.Activity
                    .Where(x => x.ActivityId.Equals(activityId))
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Comment.OrderByDescending(x => x.Datetime).Where(c => c.ReplyId == null))
                        .ThenInclude(x => x.InverseReply.OrderByDescending(x => x.Datetime)).ThenInclude(x => x.User)
                    .Include(x => x.Fanpage)
                    .Include(x => x.User)
                    .Include(x => x.Like.Where(a => a.Status))
                        .ThenInclude(x => x.User)
                    .Include(x => x.Process.OrderBy(x => x.ProcessNo).Where(x => x.Status))
                        .ThenInclude(x => x.Media)
                    .Include(x => x.Process.OrderBy(x => x.ProcessNo).Where(x => x.Status))
                        .ThenInclude(x=>x.FollowJoinAvtivity.Where(x=>x.IsJoin.Equals("success")))
                            .ThenInclude(x=>x.User)
                    .Include(x => x.Donation)
                        .ThenInclude(x=>x.User)
                    .Include(x => x.ActivityResult)
                    .Include(x => x.FollowJoinAvtivity)
                        .ThenInclude(x=>x.User)
                    .Include(x => x.Media)
                    .Include(x => x.BankAccount)
                    .FirstOrDefaultAsync();

                result.activity = activity;
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

        public async Task<ActivityResult> update(ActivityResultUpdateDTO dto)
        {
            try
            {
                var check = await this._context.ActivityResult.Where(x => x.ResultId.Equals(dto.ResultId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Title = dto.Title;
                    check.Desciption = dto.Desciption;
                    check.TotalAmount = dto.TotalAmount;
                    check.ResultDocument = dto.ResultDocument;

                    this._context.ActivityResult.Update(check);
                    await this._context.SaveChangesAsync();
                    return check;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
