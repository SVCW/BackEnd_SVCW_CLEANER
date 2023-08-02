using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Process
{
    public class updateProcessDTO
    {
        public string ProcessId { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string ProcessTitle { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ProcessTypeId { get; set; }
    }
}
