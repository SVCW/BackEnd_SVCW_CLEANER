using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Fanpage
{
    public class FollowFanpageDTO
    {
        public string UserId { get; set; }
        public string FanpageId { get; set; }
    }
}
