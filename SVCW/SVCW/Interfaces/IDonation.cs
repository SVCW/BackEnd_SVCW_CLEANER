using SVCW.DTOs.Donations;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IDonation
    {
        Task<List<Donation>> GetDonation();
        Task<List<Donation>> GetDonationsActivity(string id);
        Task<List<Donation>> GetDonationsUser(string id);
        Task<Donation> CreateDonation(DonationDTO dto);
    }
}
