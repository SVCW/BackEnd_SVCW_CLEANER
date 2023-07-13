using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.ActivityResults;
using SVCW.Interfaces;
using SVCW.Models;

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
                
                await this._context.ActivityResult.AddAsync(activityResult);
                if(await this._context.SaveChangesAsync() >0)
                {
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
                var check = await this._context.ActivityResult.ToListAsync();
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
                var check = await this._context.ActivityResult.Where(x=>x.ActivityId.Equals(activityId)).ToListAsync();
                return check;
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
