using System.ComponentModel.DataAnnotations;
using SVCW.Models;

namespace SVCW.DTOs.Users.Req
{
	public class CreateUserReq
	{
        [Required]
        public User user;
        public string Email { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public bool? Gender { get; set; }
        public string? Image { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public DateTime? CreateAt { get; set; }
        public string? Status { get; set; }
        public string? RoleId { get; set; }
        public string? coverImage { get; set; }
    }
}
