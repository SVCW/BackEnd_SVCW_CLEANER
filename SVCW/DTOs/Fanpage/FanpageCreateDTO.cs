using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Fanpage
{
    public class FanpageCreateDTO
    {
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string FanpageName { get; set; }
        public string Avatar { get; set; }
        public string CoverImage { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Description { get; set; }
        public string Mst { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Phone]
        public string Phone { get; set; }
        public string userId { get; set; }
    }
}
