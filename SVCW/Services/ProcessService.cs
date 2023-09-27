using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Config;
using SVCW.DTOs.Process;
using SVCW.DTOs.ProcessTypes;
using SVCW.Interfaces;
using SVCW.Models;


namespace SVCW.Services
{
    public class ProcessService : IProcess
    {
        private readonly SVCWContext _context;
        public ProcessService(SVCWContext context)
        {
            _context = context;
        }

        public async Task<bool> DeleteProcess(string processId)
        {
            try
            {
                var check = await this._context.Process.Where(x => x.ProcessId.Equals(processId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Status = false;
                    this._context.Process.Update(check);
                    return await this._context.SaveChangesAsync() >0;
                }
                else
                {
                    throw new Exception("not have data");
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Process>> GetAllProcess()
        {
            try
            {
                var check = await this._context.Process
                    .Include(x=>x.Media)
                    .Include(x=>x.Activity)
                    .Include(x=>x.ProcessType)
                    .OrderBy(x=>x.ProcessNo)
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("not have data");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Process> GetProcessById(string? processId)
        {
            try
            {
                var check = await this._context.Process
                    .Where(x => x.ActivityId.Equals(processId))
                    .Include(x => x.Media)
                    .Include(x => x.Activity)
                    .Include(x => x.ProcessType)
                    .FirstOrDefaultAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("not have data");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Process>> GetProcessForActivity(string activityId)
        {
            try
            {
                var check = await this._context.Process
                    .Where(x=>x.ActivityId.Equals(activityId) && x.Status)
                    .Include(x => x.Media)
                    .Include(x => x.Activity)
                    .Include(x => x.ProcessType)
                    .OrderBy(x => x.ProcessNo)
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("not have data");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Process> InsertProcess(CreateProcessDTO process)
        {
            try
            {
                var check = await this._context.Activity.Where(x => x.ActivityId.Equals(process.ActivityId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    if(process.StartDate < check.CreateAt && process.EndDate > check.EndDate)
                    {
                        throw new Exception("datetime is not valid");
                    }
                }

                var data = new Process();
                data.ProcessId = "PRC"+Guid.NewGuid().ToString().Substring(0,7);
                data.ProcessTitle = process.ProcessTitle;
                data.Description = process.Description;
                data.Status = true; 
                data.Datetime = DateTime.Now;
                data.StartDate = process.StartDate;
                data.EndDate = process.EndDate;
                data.ActivityId = process.ActivityId;
                data.ProcessTypeId = process.ProcessTypeId;
                data.ActivityResultId = null;
                data.ProcessNo = process.ProcessNo;
                data.IsKeyProcess = process.IsKeyProcess;
                data.Location = process.Location;
                if(process.IsDonateProcess == true)
                {
                    data.IsDonateProcess = true;
                    data.RealDonation= 0;
                    data.TargetDonation = process.TargetDonation;
                    check.TargetDonation += process.TargetDonation;
                    this._context.Activity.Update(check);
                    await this._context.SaveChangesAsync();
                }

                if(process.IsParticipant == true)
                {
                    data.IsParticipant = true;
                    data.RealParticipant = 0;
                    data.TargetParticipant = process.TargetParticipant;
                }

                await this._context.Process.AddAsync(data);
                if(await this._context.SaveChangesAsync() > 0)
                {
                    foreach (var media in process.media)
                    {
                        var media2 = new Media();
                        media2.MediaId = "MED" + Guid.NewGuid().ToString().Substring(0, 7);
                        media2.Type = media.Type;
                        media2.LinkMedia = media.LinkMedia;
                        media2.ProcessId = data.ProcessId;
                        await this._context.Media.AddAsync(media2);
                        await this._context.SaveChangesAsync();
                        media2 = new Media();
                    }
                    return data;
                }
                else { throw new Exception("Fail add data"); }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Process>> InsertProcessList(List<CreateProcessDTO> process)
        {
            try
            {
                var ad = new adminConfig();
                var config = new ConfigService();
                ad = config.GetAdminConfig();
                var list = new List<Process>();
                decimal donate = 0;
                foreach(var item in process)
                {
                    var actmp = await this._context.Activity.Where(x => x.ActivityId.Equals(item.ActivityId)).FirstOrDefaultAsync();
                    if (item.StartDate.Date < actmp.CreateAt.Date && item.EndDate.Date > actmp.EndDate.Date)
                    {
                        throw new Exception("Ngày tháng không hợp lệ");
                    }
                    if (item.ProcessTypeId.Equals("pt001"))
                    {
                        var ac = await this._context.Activity.Where(x => x.ActivityId.Equals(item.ActivityId)).FirstOrDefaultAsync();
                        var user = await this._context.User.Where(x=>x.UserId.Equals(ac.UserId)).FirstOrDefaultAsync();
                        if(ac.FanpageId == null)
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
                            if (item.TargetDonation > donate)
                            {
                                throw new Exception("Số tiền quyên góp tối đa bạn có thể kêu gọi là: " + donate);
                            }
                        }
                    }

                }
                foreach (var p in process)
                {
                    var data = new Process();
                    data.ProcessId = "PRC" + Guid.NewGuid().ToString().Substring(0, 7);
                    data.ProcessTitle = p.ProcessTitle;
                    data.Description = p.Description;
                    data.Status = true;
                    data.Datetime = DateTime.Now;
                    data.StartDate = p.StartDate ;
                    data.EndDate = p.EndDate;
                    data.ActivityId = p.ActivityId;
                    data.ProcessTypeId = p.ProcessTypeId;
                    data.ActivityResultId = null;
                    data.ProcessNo = p.ProcessNo;
                    data.IsKeyProcess = p.IsKeyProcess;
                    data.Location = p.Location ?? "none";
                    if (p.IsDonateProcess == true)
                    {
                        data.IsDonateProcess = true;
                        data.RealDonation = 0;
                        data.TargetDonation = p.TargetDonation;
                        var check = await this._context.Activity.Where(x => x.ActivityId.Equals(p.ActivityId)).FirstOrDefaultAsync();
                        check.TargetDonation += p.TargetDonation;
                        this._context.Activity.Update(check);
                        await this._context.SaveChangesAsync();
                    }

                    if (p.IsParticipant == true)
                    {
                        data.IsParticipant = true;
                        data.RealParticipant = 0;
                        data.TargetParticipant = p.TargetParticipant;
                    }

                    await this._context.Process.AddAsync(data);
                    if (await this._context.SaveChangesAsync() > 0)
                    {
                        foreach (var media in p.media)
                        {
                            var media2 = new Media();
                            media2.MediaId = "MED" + Guid.NewGuid().ToString().Substring(0, 7);
                            media2.Type = media.Type;
                            media2.LinkMedia = media.LinkMedia;
                            media2.ProcessId = data.ProcessId;
                            await this._context.Media.AddAsync(media2);
                            await this._context.SaveChangesAsync();
                            media2 = new Media();
                        }
                    }
                    else { throw new Exception("Fail add data"); }

                    list.Add(data);
                }
                return list;
            }catch(Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<List<Process>> SearchByTitleProcess(string? processTitle)
        {
            try
            {
                var check = await this._context.Process
                    .Where(x => x.ProcessTitle.Contains(processTitle) && x.Status)
                    .Include(x => x.Media)
                    .Include(x => x.Activity)
                    .Include(x => x.ProcessType)
                    .OrderBy(x => x.ProcessNo)
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("not have data");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Process> UpdateProcess(updateProcessDTO upProcess)
        {
            try
            {
                var check = await this._context.Process.Where(x=>x.ProcessId.Equals(upProcess.ProcessId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.ProcessTitle = upProcess.ProcessTitle;    
                    check.Description = upProcess.Description;
                    check.StartDate = upProcess.StartDate ?? check.StartDate;
                    check.EndDate = upProcess.EndDate ?? check.EndDate;
                    check.ProcessTypeId = upProcess.ProcessTypeId;
                    this._context.Process.Update(check);
                    if(await this._context.SaveChangesAsync() > 0)
                    {
                        return check;
                    }
                    else
                    {
                        throw new Exception("save fail");
                    }
                }
                else
                {
                    throw new Exception("not found process");
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
