using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Admin_Moderator.Moderator
{
    public class CreateModerator
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public bool? Gender { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        public string CoverImage { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
