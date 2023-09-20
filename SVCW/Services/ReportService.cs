using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Reports;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class ReportService : IReport
	{
        private readonly SVCWContext _context;
        public ReportService(SVCWContext context)
        {
            _context = context;
        }

        public async Task<bool> deleteReport(string rpID)
        {
            try
            {
                var db = this._context.Report.Where(rp => rp.ReportId.Equals(rpID));

                if (db != null)
                {
                    this._context.Report.RemoveRange(db);
                    this._context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Report>> getAllReport()
        {
            try
            {
                var check = await this._context.Report
                    .Include(x => x.Activity)
                        .ThenInclude(x=>x.User)
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.Fanpage)
                    .Include(x => x.ReportType)
                    .Include(x => x.User)
                    .Include(x=>x.UserReport)
                    .OrderByDescending(x => x.Datetime).ThenBy(x => x.ReportTypeId)
                    .ToListAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Report>> getAllReport2(int typeSort)
        {
            try
            {
                int date = -1;
                switch (typeSort)
                {
                    case 0:
                        var All = await this._context.Report
                                    .Include(x => x.Activity)
                                        .ThenInclude(x => x.User)
                                    .Include(x => x.Activity)
                                        .ThenInclude(x => x.Fanpage)
                                    .Include(x => x.ReportType)
                                    .Include(x => x.User)
                                    .Include(x => x.UserReport)
                                    .OrderByDescending(x => x.Datetime).ThenBy(x => x.ReportTypeId)
                                    .ToListAsync();
                        return All;
                    case 1:
                        date = -1; 
                        break;
                    case 2:
                        date = -7;
                        break;
                    case 3:
                        date = -30;
                        break;
                }
                var check = await this._context.Report
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.Fanpage)
                    .Include(x => x.ReportType)
                    .Include(x => x.User)
                    .Include(x => x.UserReport)
                    .OrderByDescending(x => x.Datetime).ThenBy(x => x.ReportTypeId)
                    .Where(x=>x.Datetime < DateTime.Now && x.Datetime > DateTime.Now.AddDays(-1))
                    .ToListAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Report>> GetReportByType(string reportType)
        {
            try
            {
                var check = await this._context.Report
                    .Where(x=>x.ReportTypeId.Equals(reportType))
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.User)
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.Fanpage)
                    .Include(x => x.ReportType)
                    .Include(x => x.User)
                    .Include(x => x.UserReport)
                    .OrderByDescending(x => x.Datetime).ThenBy(x => x.ReportTypeId)
                    .ToListAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Report> newReport(ReportDTO newReport)
        {
            try
            {
                var report = new Report();

                report.ReportId = "RPT" + Guid.NewGuid().ToString().Substring(0, 7);
                report.Title = newReport.Title;
                report.Reason = newReport.Reason;
                report.ReportTypeId = newReport.ReportTypeId;
                report.Description = newReport.Description;
                report.Status = false;
                report.UserId = newReport.UserId;
                if(newReport.ActivityId != null)
                {
                    report.ActivityId = newReport.ActivityId;
                }
                report.Datetime = DateTime.Now;
                if(newReport.UserReportId != null) 
                {
                    report.UserReportId= newReport.UserReportId;
                }

                await this._context.Report.AddAsync(report);
                await this._context.SaveChangesAsync();
                return report;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Report> updateReport(ReportDTO updatedReport)
        {
            try
            {
                var report = await this._context.Report
                    .Where(rp => updatedReport.ReportId.Equals(rp.ReportId)).FirstOrDefaultAsync();
                if (report != null)
                {
                    if (updatedReport.Title != null)
                        report.Title = updatedReport.Title;
                    if (updatedReport.Reason != null)
                        report.Reason = updatedReport.Reason;
                    if (updatedReport.ReportTypeId != null)
                        report.ReportTypeId = updatedReport.ReportTypeId;
                    if (updatedReport.Description != null)
                        report.Description = updatedReport.Description;
                    report.Status = updatedReport.Status;
                    if (updatedReport.UserId != null)
                        report.UserId = updatedReport.UserId;
                    this._context.Report.Update(report);
                    await this._context.SaveChangesAsync();
                }
                return report;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

