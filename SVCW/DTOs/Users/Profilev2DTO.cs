using SVCW.Models;

namespace SVCW.DTOs.Users
{
    public class Profilev2DTO
    {
        public User user { get; set; }
        public List<Activity> activity { get; set; }
    }
}
