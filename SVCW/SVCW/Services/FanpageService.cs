using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Fanpage;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class FanpageService : IFanpage
    {
        private readonly SVCWContext _context;
        public FanpageService(SVCWContext context)
        {
            _context = context;
        }
        public async Task<Fanpage> create(FanpageCreateDTO dto)
        {
            try
            {
                var fanpage = new Fanpage();
                fanpage.Status = "1";
                fanpage.Email = dto.Email;
                fanpage.Description = dto.Description;
                fanpage.CoverImage = dto.CoverImage;
                fanpage.FanpageName= dto.FanpageName;
                fanpage.FanpageId = dto.userId;
                fanpage.Phone = dto.Phone;
                fanpage.Avatar= dto.Avatar;
                fanpage.CreateAt = DateTime.Now;
                fanpage.Mst = dto.Mst;
                fanpage.NumberFollow = 0;

                await this._context.Fanpage.AddAsync(fanpage);
                await this._context.SaveChangesAsync();

                return fanpage;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Fanpage> delete(string fanpageID)
        {
            try
            {
                var check = await this._context.Fanpage.Where(x => x.FanpageId.Equals(fanpageID)).FirstOrDefaultAsync();
                check.Status = "0";
                await this._context.SaveChangesAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> follow(string userId, string fanpageId)
        {
            try
            {
                var check = new FollowFanpage();
                check.UserId = userId;
                check.FanpageId = fanpageId;
                check.Datetime = DateTime.Now;
                check.Status = true;

                await this._context.FollowFanpage.AddAsync(check);
                await this._context.SaveChangesAsync();

                var fanpage = await this._context.Fanpage.Where(x => x.FanpageId.Equals(fanpageId)).FirstOrDefaultAsync();
                fanpage.NumberFollow += 1;
                this._context.Fanpage.Update(fanpage);
                return await this._context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Fanpage>> getAll()
        {
            try
            {
                var check = await this._context.Fanpage
                    .Include(x=>x.Activity)
                        .ThenInclude(x => x.Comment.Where(b => b.ReplyId == null))
                            .ThenInclude(x => x.InverseReply)
                            .ThenInclude(x => x.User)
                        .ThenInclude(x => x.Comment.Where(b => b.ReplyId == null))
                            .ThenInclude(x => x.User)
                        .ThenInclude(x => x.Comment.OrderByDescending(x => x.Datetime))
                    .Include(x=>x.FollowFanpage)
                    .Include(x=>x.FanpageNavigation)
                    .ToListAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Fanpage> getByID(string id)
        {
            try
            {
                var check = await this._context.Fanpage.Where(x=>x.FanpageId.Equals(id))
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.Comment.Where(b => b.ReplyId == null))
                            .ThenInclude(x => x.InverseReply)
                            .ThenInclude(x => x.User)
                        .ThenInclude(x => x.Comment.Where(b => b.ReplyId == null))
                            .ThenInclude(x => x.User)
                        .ThenInclude(x => x.Comment.OrderByDescending(x => x.Datetime))
                    .Include(x => x.FollowFanpage)
                    .Include(x => x.FanpageNavigation)
                    .FirstOrDefaultAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Fanpage>> getByName(string name)
        {
            try
            {
                var check = await this._context.Fanpage.Where(x=>x.FanpageName.Contains(name))
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.Comment.Where(b => b.ReplyId == null))
                            .ThenInclude(x => x.InverseReply)
                            .ThenInclude(x => x.User)
                        .ThenInclude(x => x.Comment.Where(b => b.ReplyId == null))
                            .ThenInclude(x => x.User)
                        .ThenInclude(x => x.Comment.OrderByDescending(x => x.Datetime))
                    .Include(x => x.FollowFanpage)
                    .Include(x => x.FanpageNavigation)
                    .ToListAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Fanpage>> getFotUser()
        {
            try
            {
                var check = await this._context.Fanpage.Where(x=>x.Status.Equals("2"))
                    .Include(x => x.Activity)
                        .ThenInclude(x => x.Comment.Where(b => b.ReplyId == null))
                            .ThenInclude(x => x.InverseReply)
                            .ThenInclude(x => x.User)
                        .ThenInclude(x => x.Comment.Where(b => b.ReplyId == null))
                            .ThenInclude(x => x.User)
                        .ThenInclude(x => x.Comment.OrderByDescending(x => x.Datetime))
                    .Include(x => x.FollowFanpage)
                    .Include(x => x.FanpageNavigation)
                    .ToListAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Fanpage>> getModerate()
        {
            try
            {
                var check = await this._context.Fanpage.Where(x => x.Status.Equals("1")).ToListAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Fanpage> moderate(string fanpageID)
        {
            try
            {
                var check = await this._context.Fanpage.Where(x => x.FanpageId.Equals(fanpageID)).FirstOrDefaultAsync();
                check.Status = "2";
                await this._context.SaveChangesAsync();
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> unfollow(string userId, string fanpageId)
        {
            try
            {
                var check = await this._context.FollowFanpage.Where(x=>x.UserId.Equals(userId)&&x.FanpageId.Equals(fanpageId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.Status = false;
                    this._context.FollowFanpage.Update(check);
                    await this._context.SaveChangesAsync();

                    var fanpage = await this._context.Fanpage.Where(x => x.FanpageId.Equals(fanpageId)).FirstOrDefaultAsync();
                    fanpage.NumberFollow -= 1;
                    this._context.Fanpage.Update(fanpage);
                    return await this._context.SaveChangesAsync() > 0;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Fanpage> update(FanpageUpdateDTO dto)
        {
            try
            {
                var check = await this._context.Fanpage.Where(x=>x.FanpageId.Equals(dto.FanpageId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.FanpageName = dto.FanpageName;
                    check.Avatar = dto.Avatar;
                    check.CoverImage= dto.CoverImage;
                    check.Description   = dto.Description;
                    check.Mst = dto.Mst;
                    check.Email = dto.Email;
                    check.Phone = dto.Phone;
                    this._context.Fanpage.Update(check);
                    await this._context.SaveChangesAsync();
                }
                return check;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
