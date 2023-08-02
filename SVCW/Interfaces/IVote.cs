using SVCW.DTOs.Votes;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IVote
    {
        Task<List<Vote>> getAll();
        Task<Vote> createVote(VoteDTO vote);
        Task<Vote> updateVote(VoteDTO vote);
        Task<Vote> deleteVote(VoteDTO vote);
    }
}
