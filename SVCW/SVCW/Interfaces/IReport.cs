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
	}
}

