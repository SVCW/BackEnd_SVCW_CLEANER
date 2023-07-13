using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Achivements;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class AchivementService : IAchivement
    {
        protected readonly SVCWContext context;
        public AchivementService(SVCWContext context)
        {
            this.context = context;
        }
        public async Task<bool> DeleteAchivement(List<string> achivementId)
        {
            try
            {
                List<Achivement> achivement = await this.context.Achivement
                    .Where(x => achivementId.Contains(x.AchivementId))
                    .ToListAsync();
                if (achivement != null && achivement.Count > 0)
                {

                    for (int i = 0; i < achivement.Count; i++)
                    {
                        achivement[i].Status = false;
                    }
                    await this.context.SaveChangesAsync();
                    return true;
                }
                else
                {
                    throw new ArgumentException("No Achivement found");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Process went wrong");
            }
        }

        public async Task<List<Achivement>> GetAchivementById(string? achivementId)
        {
            try
            {
                var data = await this.context.Achivement.Where(x => x.Status && x.AchivementId.Equals(achivementId)).
                    Select(x => new Achivement
                    {
                        AchivementId = x.AchivementId,
                        AchivementLogo = x.AchivementLogo,
                        Description = x.Description,
                        CreateAt = x.CreateAt,
                        Status = x.Status,
                    }).ToListAsync();
                if (data.Count > 0 && data != null)
                    return data;
                else throw new ArgumentException();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Process went wrong");
            }
        }

        public async Task<List<Achivement>> GetAllAchivements()
        {
            try
            {
                var data = await this.context.Achivement.Where(x => x.Status)
                    .Select(x => new Achivement
                    {
                       AchivementId= x.AchivementId,
                       AchivementLogo= x.AchivementLogo,
                       Description= x.Description,
                       CreateAt= x.CreateAt,
                       Status= x.Status,
                    })
                    .ToListAsync();
                return data;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public async Task<bool> InsertAchivement(AchivementDTO achivement)
        {
            try
            {
                var _achivement = new Achivement();
                _achivement.AchivementId = "AId" + Guid.NewGuid().ToString().Substring(0, 7);
                _achivement.AchivementLogo = achivement.AchivementLogo;
                _achivement.Description = achivement.Description;
                _achivement.CreateAt = DateTime.Now;
                _achivement.Status = achivement.Status;
                await this.context.Achivement.AddAsync(_achivement);
                this.context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Process went wrong");
            }
        }

        public async Task<bool> UpdateAchivement(AchivementDTO upAchivement)
        {
            try
            {
                Achivement achivement = await this.context.Achivement.FirstAsync(x => x.AchivementId == upAchivement.AchivementId);
                if (achivement != null)
                {
                    achivement.AchivementId = upAchivement.AchivementId;
                    achivement.AchivementLogo = upAchivement.AchivementLogo;
                    achivement.Description= upAchivement.Description;
                    //achivement.CreateAt= upAchivement.CreateAt;
                    achivement.Status= upAchivement.Status;
                    this.context.SaveChanges();
                    return true;

                }
                else
                {
                    throw new ArgumentException("Achivement not found");
                }
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Process went wrong");
            }
        }

        public async Task<bool> UserAchivement(string userID, string achivemnt)
        {
            try
            {
                var acus = new AchivementUser();
                acus.AchivementId = achivemnt;
                acus.UserId = userID;
                acus.EndDate = DateTime.MaxValue;

                await this.context.AchivementUser.AddAsync(acus);

                return await this.context.SaveChangesAsync()>0;
            }catch(Exception ex) { throw new Exception(ex.Message); }
        }
    }
}
