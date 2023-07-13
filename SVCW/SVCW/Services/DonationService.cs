using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Donations;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class DonationService : IDonation
    {
        protected readonly SVCWContext context;
        public DonationService(SVCWContext context)
        {
            this.context = context;
        }

        public async Task<Donation> CreateDonation(DonationDTO dto)
        {
            try
            {
                var donate = new Donation();
                donate.DonationId = "DNT"+Guid.NewGuid().ToString().Substring(0,7);
                donate.Title= dto.Title;
                donate.Datetime = DateTime.Now;
                donate.Amount= dto.Amount;
                donate.Email=dto.Email;
                donate.Phone=dto.Phone;
                donate.Name= dto.Name;
                donate.IsAnonymous= dto.IsAnonymous;
                donate.Status = "đã tạo, chưa thanh toán";
                donate.ActivityId = dto.ActivityId;
                donate.TaxVnpay = null;
                var check = await this.context.User.Where(x=>x.Email.Equals(dto.Email)).FirstOrDefaultAsync();
                if (check != null)
                {
                    donate.UserId= check.UserId;
                }
                else
                {
                    donate.UserId= null;
                }
                donate.PayDate = null;
                await this.context.Donation.AddAsync(donate);
                if(await this.context.SaveChangesAsync() >0)
                {
                    return donate;
                }
                else
                {
                    throw new Exception("Lỗi trong quá trình tạo quyên góp");
                }
                
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Donation>> GetDonation()
        {
            try
            {
                var check = await this.context.Donation.ToListAsync();
                if(check !=null)
                {
                    return check;
                }return null;
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Donation>> GetDonationsActivity(string id)
        {
            try
            {
                var check = await this.context.Donation.Where(x=>x.ActivityId.Equals(id)).ToListAsync();
                if (check != null)
                {
                    return check;
                }
                return null;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
