using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.ProcessTypes
{
    public class ProcessTypeDTO
    {
        [Key]
        [Column("processTypeId")]
        [StringLength(10)]
        public string ProcessTypeId { get; set; }
        [Required]
        [Column("processTypeName")]
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string ProcessTypeName { get; set; }
        [Required]
        [Column("description")]
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Description { get; set; }
    }
}
