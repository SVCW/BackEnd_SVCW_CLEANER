using System;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Reports
{
	public class ReportDTO
	{
		public string? ReportId { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Title { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Reason { get; set; }
		public string ReportTypeId { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Description { get; set; }
		public bool Status { get; set; }
		public string UserId { get; set; }
        public string? UserReportId { get; set; }
        public string? ActivityId { get; set; }
    }
}

