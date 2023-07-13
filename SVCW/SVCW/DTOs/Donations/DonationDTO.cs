using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Donations
{
    public class DonationDTO
    {
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Title { get; set; }
        public decimal Amount { get; set; }
        [EmailAddress]   
        public string Email { get; set; }
        [Phone]          
        public string Phone { get; set; }       
        public string Name { get; set; }
        public bool? IsAnonymous { get; set; }        
        public string ActivityId { get; set; }
    }
}
