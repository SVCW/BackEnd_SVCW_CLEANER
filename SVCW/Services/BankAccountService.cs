using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.BankAccount;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class BankAccountService : IBankAccount
    {
        private readonly SVCWContext _context;
        public BankAccountService(SVCWContext context)
        {
            _context = context;
        }
        public async Task<BankAccount> create(BankAccountDTO dto)
        {
            try
            {
                var bankAccount = new BankAccount();
                bankAccount.BankAccountId = "BKACC" + Guid.NewGuid().ToString().Substring(0, 5);
                bankAccount.Status = true;
                bankAccount.BankAccountName = dto.BankAccountName;
                bankAccount.BankNumber = dto.BankNumber;
                bankAccount.BankName = dto.BankName;
                bankAccount.Description = dto.Description;
                bankAccount.UserId = dto.UserId;

                await this._context.BankAccount.AddAsync(bankAccount);
                await this._context.SaveChangesAsync();

                return bankAccount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            
        }

        public async Task<List<BankAccount>> getAll()
        {
            try
            {
                var check = await this._context.BankAccount.ToListAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<BankAccount>> getForUser(string userId)
        {
            try
            {
                var check = await this._context.BankAccount.Where(x=>x.UserId.Equals(userId)).ToListAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> updateBankForActivity(string bankIdOld, string activityId, string newBankId)
        {
            try
            {
                var check = await this._context.BankAccount.Where(x=>x.BankAccountId.Equals(bankIdOld)).FirstOrDefaultAsync();
                var check1 = await this._context.Activity.Where(x=>x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                var check2 = await this._context.BankAccount.Where(x => x.BankAccountId.Equals(newBankId)).FirstOrDefaultAsync();

                check.Activity.Remove(check1);
                check2.Activity.Add(check1);

                return await this._context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> useBankForActivity(string bankId, string activityId)
        {
            try
            {
                var check = await this._context.BankAccount.Where(x=>x.BankAccountId.Equals(bankId)).FirstOrDefaultAsync();
                var check1 = await this._context.Activity.Where(x => x.ActivityId.Equals(activityId)).FirstOrDefaultAsync();
                check.Activity.Add(check1);
                this._context.Update(check);
                return await this._context.SaveChangesAsync()>0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
