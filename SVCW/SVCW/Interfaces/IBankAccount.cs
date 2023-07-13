using SVCW.DTOs.BankAccount;
using SVCW.Models;

namespace SVCW.Interfaces
{
    public interface IBankAccount
    {
        Task<BankAccount> create(BankAccountDTO dto);
        Task<List<BankAccount>> getAll();
        Task<List<BankAccount>> getForUser(string userId);
        Task<bool> useBankForActivity(string bankId,string activityId);
        Task<bool> updateBankForActivity(string bankIdOld,string activityId, string newBankId);
    }
}
