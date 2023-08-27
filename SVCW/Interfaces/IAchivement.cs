using SVCW.DTOs.Achivements;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IAchivement
    {
        Task<List<Achivement>> GetAchivementById(string? achivementId);
        Task<List<Achivement>> GetAllAchivements();
        Task<bool> UpdateAchivement(AchivementDTO upAchivement);
        Task<bool> DeleteAchivement(string achivementId);
        Task<bool> InsertAchivement(AchivementDTO achivement);
        Task<bool> UserAchivement(string userID, string achivemnt);
    }
}
