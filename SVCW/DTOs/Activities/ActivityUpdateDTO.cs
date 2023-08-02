using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Activities
{
    public class ActivityUpdateDTO
    {
        public string ActivityId { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Title { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Location { get; set; }        
        public decimal? TargetDonation { get; set; }
    }
}
