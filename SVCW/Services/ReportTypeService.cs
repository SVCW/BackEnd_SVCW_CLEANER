using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.ReportTypes;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class ReportTypeService : IReportType
    {
        protected readonly SVCWContext context;
        public ReportTypeService(SVCWContext context)
        {
            this.context = context;
        }
        public async Task<bool> DeleteReportType(List<string> reportTypeId)
        {
            try
            {
                List<ReportType> reportTypes = await this.context.ReportType
                    .Where(x => reportTypeId.Contains(x.ReportTypeId))
                    .ToListAsync();
                if (reportTypes != null && reportTypes.Count > 0)
                {

                    for (int i = 0; i < reportTypes.Count; i++)
                    {
                        reportTypes[i].Status = false;
                        this.context.ReportType.Update(reportTypes[i]);
                    }
                    
                    await this.context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new ArgumentException("No ReportType found");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Process went wrong");
            }
        }

        public async Task<List<ReportType>> GetAllReportTypes()
        {
            try
            {

                var reportType = await this.context.ReportType.Where(x => x.Status)
                    .Select(x => new ReportType
                {
                    ReportTypeId = x.ReportTypeId,
                    ReportTypeName = x.ReportTypeName,
                    Status= x.Status
                }).ToListAsync();
                return reportType;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Process went wrong");
            }
        }

        public async Task<ReportType> GetReportTypeById(string? reportTypeId)
        {
            try
            {
                var data = await this.context.ReportType.Where(x => x.Status && x.ReportTypeId == reportTypeId)
                    .Select(x => new ReportType()
                {
                        ReportTypeId = x.ReportTypeId,
                        ReportTypeName = x.ReportTypeName,
                        Status = x.Status
                    }).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("process wrong connect");
            }

        }

        public async Task<bool> InsertReportType(ReportTypeDTO reportType)
        {
            try
            {
                ReportType _reportType = new ReportType();

                _reportType.ReportTypeId = "RId" + Guid.NewGuid().ToString().Substring(0, 7);
                _reportType.ReportTypeName = reportType.ReportTypeName;
                _reportType.Status = reportType.Status;
                await this.context.ReportType.AddAsync(_reportType);
                this.context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                if (ex.InnerException.Message.Contains("duplicate"))
                {
                    throw new Exception("Loại báo cáo đã có trong hệ thống");
                }
                throw new Exception(ex.Message);
            }   
        }

        public async Task<ReportType> SearchByNameReportType(string? reportTypeName)
        {
            try
            {
                var data = await this.context.ReportType.Where(x => x.Status && x.ReportTypeName.Contains(reportTypeName))
                    .Select(x => new ReportType()
                    {
                        ReportTypeId = x.ReportTypeId,
                        ReportTypeName = x.ReportTypeName,
                        Status = x.Status
                    }).FirstOrDefaultAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception("process wrong connect");
            }
        }

        public async Task<bool> UpdateReportType(ReportTypeDTO upReportType)
        {
            try
            {
                ReportType _reportType = await this.context.ReportType
                    .FirstOrDefaultAsync(x => x.ReportTypeId == upReportType.ReportTypeId);
                if (_reportType != null)
                {
                    _reportType.ReportTypeId = upReportType.ReportTypeId;
                    _reportType.ReportTypeName = upReportType.ReportTypeName;
                    _reportType.Status = upReportType.Status;
                    this.context.ReportType.Update(_reportType);
                    this.context.SaveChanges();
                    return true;
                }
                else
                {
                    throw new Exception("Not Found Report Type!");
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
