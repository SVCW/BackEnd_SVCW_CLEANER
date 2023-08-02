namespace SVCW.DTOs.Admin_Moderator.Moderator
{
    public class updateModerator
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Image { get; set; }
        public bool? Gender { get; set; }
        public string FullName { get; set; }
        public string CoverImage { get; set; }
        public DateTime? DateOfBirth { get; set; }
    }
}
