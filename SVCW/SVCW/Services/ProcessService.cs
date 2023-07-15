using Microsoft.EntityFrameworkCore;
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
                var data = new Process();
                data.ProcessId = "PRC"+Guid.NewGuid().ToString().Substring(0,7);
                data.ProcessTitle = process.ProcessTitle;
                data.Description = process.Description;
                data.Status = true; 
                data.Datetime = DateTime.Now;
                data.StartDate = process.StartDate ?? null;
                data.EndDate = process.EndDate ?? null;
                data.ActivityId = process.ActivityId;
                data.ProcessTypeId = process.ProcessTypeId;
                data.ActivityResultId = null;

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

        public async Task<List<Process>> SearchByTitleProcess(string? processTitle)
        {
            try
            {
                var check = await this._context.Process
                    .Where(x => x.ProcessTitle.Contains(processTitle) && x.Status)
                    .Include(x => x.Media)
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
