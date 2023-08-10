
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Users.Req
{
	public class UpdateUserReq
	{
        public string UserId { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public bool? Gender { get; set; }
        public string? Image { get; set; }
        public string? CoverImage { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Status { get; set; }
        public string? RoleId { get; set; }
    }
}
