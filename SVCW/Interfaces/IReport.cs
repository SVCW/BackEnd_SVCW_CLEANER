using System;
using SVCW.DTOs.Reports;
using SVCW.Models;

namespace SVCW.Interfaces
{
	public interface IReport
	{
		Task<List<Report>> getAllReport();
		Task<Report> newReport(ReportDTO newReport);
		Task<Report> updateReport(ReportDTO updatedReport);
		Task<bool> deleteReport(string rpID);
		Task<List<Report>> GetReportByType(string reportType);
		Task<List<Report>> getAllReport2(int typeSort);

    }
}

