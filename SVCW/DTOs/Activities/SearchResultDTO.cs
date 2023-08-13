using SVCW.Models;

namespace SVCW.DTOs.Activities
{
    public class SearchResultDTO
    {
        public List<Activity> activities { get; set; }
        public List<User> users { get; set; }
        public List<Models.Fanpage> fanpages { get; set; }
    }
}
