using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.ActivityResults
{
    public class ActivityResultCreateDTO
    {
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Title { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Desciption { get; set; }
        public string ActivityId { get; set; }
    }
}
