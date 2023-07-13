using SVCW.DTOs.Fanpage;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IFanpage
    {
        Task<bool> follow(string userId, string fanpageId);
        Task<bool> unfollow(string userId, string fanpageId);
        Task<Fanpage> create(FanpageCreateDTO dto);
        Task<Fanpage> update(FanpageUpdateDTO dto);
        Task<List<Fanpage>> getAll();
        Task<List<Fanpage>> getModerate();
        Task<Fanpage> getByID(string id);
        Task<List<Fanpage>> getByName(string name);
        Task<List<Fanpage>> getFotUser();
        Task<Fanpage> moderate(string fanpageID);
        Task<Fanpage> delete(string fanpageID);
    }
}
