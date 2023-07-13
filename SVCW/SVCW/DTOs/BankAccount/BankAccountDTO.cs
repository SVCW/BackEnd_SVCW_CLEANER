using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.BankAccount
{
    public class BankAccountDTO
    {
        public string BankAccountName { get; set; }
        public string BankNumber { get; set; }
        public string BankName { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Description { get; set; }
        public bool Status { get; set; }
        public string UserId { get; set; }
    }
}
