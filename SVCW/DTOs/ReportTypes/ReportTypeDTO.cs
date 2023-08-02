using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.ReportTypes
{
    public class ReportTypeDTO
    {
        [Key]
        [Column("reportTypeId")]
        [StringLength(10)]
        public string ReportTypeId { get; set; }
        [Required]
        [Column("reportTypeName")]
        public string ReportTypeName { get; set; }
        [Column("status")]
        public bool Status { get; set; }
    }
}
