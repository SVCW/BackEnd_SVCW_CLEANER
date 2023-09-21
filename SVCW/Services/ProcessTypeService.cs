using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.ProcessTypes;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
 
    public class ProcessTypeService : IProcessType
    {
        protected readonly SVCWContext context;
        public ProcessTypeService(SVCWContext context)
        {
            this.context = context;
        }
        public async Task<bool> DeleteProcessType(List<string> processTypeId)
        {
            try
            {
                var processType = await this.context.ProcessType
                    .Where(x => processTypeId.Contains(x.ProcessTypeId)).ToListAsync();
                if (processType != null && processType.Count() >= 1)
                {
                    this.context.RemoveRange(processType);
                    this.context.SaveChanges();
                    return true;

                }
                else
                {
                    throw new Exception("Not Found Process Type Type!");
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<ProcessType>> GetAllProcessType()
        {
            try
            {
                var data = await this.context.ProcessType.ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public async Task<ProcessType> GetProcessTypeById(string? processTypeId)
        {
            try
            {
                var data = await this.context.ProcessType.Where(x => x.ProcessTypeId == processTypeId)
                    .Select(x => new ProcessType()
                    {
                        ProcessTypeId = x.ProcessTypeId,
                        ProcessTypeName = x.ProcessTypeName,
                        Description = x.Description
                    }).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("process wrong connect");
            }
        }

        public async Task<bool> InsertProcessType(ProcessTypeDTO processType)
        {
            try
            {
                var _processType = new ProcessType();
               _processType.ProcessTypeId = "PId" + Guid.NewGuid().ToString().Substring(0, 7);
                _processType.ProcessTypeName = processType.ProcessTypeName;
                _processType.Description= processType.Description;
                await this.context.ProcessType.AddAsync(_processType);
                this.context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.Contains("duplicate"))
                {
                    throw new Exception("Loại hoạt động đã có trong hệ thống");
                }
                throw new Exception(ex.Message);
            }
        }

        public async Task<ProcessType> SearchByNameProcessType(string? processTypeName)
        {
            try
            {
                var data = await this.context.ProcessType.Where(x => x.ProcessTypeName.Contains(processTypeName))
                    .Select(x => new ProcessType()
                    {
                        ProcessTypeId = x.ProcessTypeId,
                        ProcessTypeName = x.ProcessTypeName,
                        Description = x.Description
                    }).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("process wrong connect");
            }
        }

        public async Task<bool> UpdateProcessType(ProcessTypeDTO upProcessType)
        {
            try
            {
                ProcessType processType = await this.context.ProcessType
                    .FirstOrDefaultAsync(x => x.ProcessTypeId == upProcessType.ProcessTypeId);
                if (processType != null)
                {
                    processType.ProcessTypeName = upProcessType.ProcessTypeName;
                    processType.Description = upProcessType.Description;
                    this.context.ProcessType.Update(processType);
                    this.context.SaveChanges();
                    return true;

                }
                else
                {
                    throw new Exception("Not Found Food Type!");
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
