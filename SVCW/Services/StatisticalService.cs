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
                var donate = await this.context.Donation.Where(x => x.UserId.Equals(userId) && x.PayDate >= start && x.PayDate <= end && x.Status.Equals("success")).ToListAsync();
                result.TotalDonate = 0;
                foreach( var donation in donate)
                {
                    result.TotalDonate += donation.Amount;
                }

                result.totalNumberActivityCreate = 0;
                result.Donated = 0;
                var donated = await this.context.Activity.Where(x => x.UserId.Equals(userId) && x.CreateAt >= start && x.CreateAt <= end && x.Status.Equals("Active")).ToListAsync();
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

        public async Task<StatisticalDonateDTO> getDonateByTime(DateTime start, DateTime end)
        {
            try
            {
                var result = new StatisticalDonateDTO();
                result.numberCreateActivity= 0;
                result.total = 0;
                result.target = 0;
                result.start = start;
                result.end = end;
                var ac = await this.context.Activity.Where(x=>x.CreateAt >= start && x.CreateAt <= end).ToListAsync();
                foreach(var x in ac)
                {
                    result.numberCreateActivity ++;
                    result.total += (decimal)x.RealDonation;
                    var pro = await this.context.Process.Where(p=>p.ActivityId.Equals(x.ActivityId) && p.ProcessTypeId.Equals("pt001") && x.TargetDonation != null).ToListAsync();
                    foreach(var p in pro)
                    {

                        result.target += (decimal)p.TargetDonation;
                    }
                }
                return result;
            }catch(Exception ex )
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<StatisticalActivityDTO> getActivityBytime(DateTime start, DateTime end)
        {
            try
            {
                var result = new StatisticalActivityDTO();
                result.start = start;
                result.end = end;
                result.numberReport = 0;
                result.numberLike= 0;
                result.numberJoin= 0;
                result.numberDonate= 0;
                result.numberComment= 0;
                result.count= 0;
                var ac = await this.context.Activity.Where(x => x.CreateAt >= start && x.CreateAt <= end).ToListAsync();
                foreach( var p in ac)
                {
                    result.count++;

                    var cmt = await this.context.Comment.Where(x=>x.ActivityId.Equals(p.ActivityId)).ToListAsync();
                    foreach(var c in cmt)
                    {
                        result.numberComment++;
                    }

                    var join = await this.context.FollowJoinAvtivity.Where(x => x.ActivityId.Equals(p.ActivityId) && x.IsJoin.Equals("success")).ToListAsync();
                    foreach(var j in join)
                    {
                        result.numberJoin++;
                    }

                    var rpt = await this.context.Report.Where(x => x.ActivityId.Equals(p.ActivityId)).ToListAsync();
                    foreach(var r in rpt)
                    {
                        result.numberReport++;
                    }

                    var donate = await this.context.Donation.Where(x=>x.ActivityId.Equals(p.ActivityId) && x.Status.Equals("success")).ToListAsync();
                    foreach(var d in donate)
                    {
                        result.numberDonate++;
                    }

                    var like = await this.context.Like.Where(x=>x.ActivityId.Equals(p.ActivityId) && x.Status).ToListAsync();
                    foreach(var l in like)
                    {
                        result.numberLike++;
                    }
                }

                return result;
            }catch(Exception ex) { throw new Exception(ex.Message); }
        }

        public async Task<StatisticalAdminDTO> getByTime(DateTime start, DateTime end)
        {
            try
            {
                var result = new StatisticalAdminDTO();
                result.start = start;
                result.end = end;
                result.newMor = 0;
                result.newUser = 0;
                var check = await this.context.User.Where(x=>x.CreateAt <= end && x.CreateAt >= start).ToListAsync();
                foreach(var item in check)
                {
                    if (item.UserId.Contains("MDR"))
                    {
                        result.newMor++;
                    }
                    if (item.UserId.Contains("USR"))
                    {
                        result.newUser++;
                    }
                }
                return result;
            }catch(Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
