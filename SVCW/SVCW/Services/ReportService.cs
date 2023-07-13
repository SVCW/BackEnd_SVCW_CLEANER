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
                var check = await this._context.Report.ToListAsync();
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

