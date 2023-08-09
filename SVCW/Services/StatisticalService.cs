using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Statistical;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class StatisticalService : IStatistical
    {
        protected readonly SVCWContext context;
        public StatisticalService(SVCWContext context)
        {
            this.context = context;
        }
        public async Task<StatisticalUserDonateDTO> get(string userId, DateTime start, DateTime end)
        {
            try
            {
                var result = new StatisticalUserDonateDTO();
                var donate = await this.context.Donation.Where(x => x.UserId.Equals(userId) && x.PayDate >= start && x.PayDate <= end).ToListAsync();
                result.TotalDonate = 0;
                foreach( var donation in donate)
                {
                    result.TotalDonate += donation.Amount;
                }

                result.totalNumberActivityCreate = 0;
                result.Donated = 0;
                var donated = await this.context.Activity.Where(x => x.UserId.Equals(userId) && x.CreateAt >= start && x.CreateAt <= end).ToListAsync();
                foreach(var x in donated)
                {
                    result.Donated += x.RealDonation;
                    result.totalNumberActivityCreate += 1;
                }
                return result;
                
            }catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
