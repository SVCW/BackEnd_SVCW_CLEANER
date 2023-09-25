using Microsoft.EntityFrameworkCore;
using SixLabors.ImageSharp;
using SVCW.DTOs.Donations;
using SVCW.Interfaces;
using SVCW.Models;
using System.Globalization;

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
                var activity = await this.context.Activity.Where(x => x.ActivityId.Equals(dto.ActivityId)).FirstOrDefaultAsync();
                if(activity.Status.Equals("Pending"))
                {
                    throw new Exception("Chiến dịch chưa được duyệt");
                }
                if (activity.Status.Equals("Reject"))
                {
                    throw new Exception("Chiến dịch bị từ chối");
                }
                if (activity.Status.Equals("Quit"))
                {
                    throw new Exception("Chiến dịch đã bị chủ sở hữu hủy sớm nên bạn không thể ủng hộ");
                }
                var pro = await this.context.Process.Where(x=>x.ActivityId.Equals(dto.ActivityId)).ToListAsync();
                foreach (var p in pro)
                {
                    if (p.ProcessTypeId.Equals("pt001"))
                    {
                        if(DateTime.Now < p.StartDate || DateTime.Now >= p.EndDate)
                        {
                            throw new Exception("Bạn chưa thể quyên góp cho chiến dịch này");
                        }
                        if((p.RealDonation + dto.Amount) > p.TargetDonation)
                        {
                            decimal d = ((decimal)(p.TargetDonation - p.RealDonation));
                            string formattedNumber = d.ToString("#,##0", CultureInfo.InvariantCulture);
                            throw new Exception("Chiến dịch chỉ còn thiếu: " + formattedNumber +" bạn vui lòng hãy quyên góp đúng số tiền nhé");
                        }
                    }
                }
                var donate = new Donation();
                donate.DonationId = "DNT"+Guid.NewGuid().ToString().Substring(0,7);
                donate.Title= dto.Title;
                donate.Datetime = DateTime.Now;
                //donate.Amount= dto.Amount;
                donate.Amount = Decimal.Floor(dto.Amount);
                donate.Email=dto.Email;
                donate.Phone=dto.Phone;
                donate.Name= dto.Name;
                donate.IsAnonymous= dto.IsAnonymous;
                donate.Status = "Ủng hộ không thành công";
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
                var check = await this.context.Donation.Where(x=>x.Status.Equals("success")).ToListAsync();
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

        public async Task<List<Donation>> GetDonationsUser(string id)
        {
            try
            {
                var check = await this.context.Donation.Where(x=>x.UserId.Equals(id))
                    .Include(x=>x.Activity)
                    .ToListAsync();
                if (check != null)
                {
                    return check;
                }
                else
                {
                    throw new Exception("not have data");
                }
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
