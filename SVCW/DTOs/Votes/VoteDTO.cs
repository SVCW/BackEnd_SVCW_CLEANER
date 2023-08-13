using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Votes
{
    public class VoteDTO
    {
        public string? VoteId { get; set; }
        public string UserVoteId { get; set; }
        public string UserId { get; set; }
        public bool? IsLike { get; set; }
        public bool? IsDislike { get; set; }
    }
}
