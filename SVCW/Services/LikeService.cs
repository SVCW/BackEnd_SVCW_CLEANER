using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Likes;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
	public class LikeService : ILike
	{
        private readonly SVCWContext _context;

		public LikeService(SVCWContext context)
		{
            _context = context;
		}

        public Task<bool> DeleteLikeByIds(string userId, string activityId)
        {
            throw new NotImplementedException();
        }

        public async Task<List<Like>> GetActivityLikes(string activityId)
        {
            try
            {
                var likes = await this._context.Like.Where(x => x.ActivityId.Equals(activityId)).ToListAsync();
                return likes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Like>> InsertLike(List<LikeDTO> listLikes)
        {
            try
            {
                var _listLikes = new List<Like>();
                var _like = new Like();
                foreach (var like in listLikes)
                {
                    _like.UserId = like.UserId;
                    _like.ActivityId = like.ActivityId;
                    _like.Datetime = DateTime.Now;
                    _like.Status = true;
                    await this._context.Like.AddAsync(_like);
                    this._context.SaveChanges();



                    _listLikes.Add(_like);
                }
                return _listLikes;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> SimpleLike(LikeDTO likeInfo)
        {
            try
            {
                var _like = new Like();

                _like.UserId = likeInfo.UserId;
                _like.ActivityId = likeInfo.ActivityId;
                _like.Datetime = DateTime.Now;
                _like.Status = true;
                await this._context.Like.AddAsync(_like);
                this._context.SaveChanges();

                var check = await this._context.Activity.Where(x=>x.ActivityId.Equals(likeInfo.ActivityId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    check.NumberLike += 1;
                }
                this._context.Activity.Update(check);
                await this._context.SaveChangesAsync();


                var noti = new Notification();
                // ai like 
                var userlike = await this._context.User.Where(x=>x.UserId.Equals(_like.UserId)).FirstOrDefaultAsync();
                // kiểm tra chủ sở hữu tự like chiến dịch
                if (!userlike.UserId.Equals(check.UserId)) 
                {
                    // noti cho chủ sở hữu
                    noti = new Notification();
                    if (userlike.FullName.Equals("none"))
                    {
                        noti.Title = userlike.Username + " đã thích chiến dịch của bạn";
                    }
                    else
                    {
                        noti.Title = userlike.FullName + " đã thích chiến dịch của bạn";
                    }
                    noti.NotificationContent = "Đã có tình nguyện viên thích chiến dịch của bạn";
                    noti.Datetime = DateTime.Now;
                    noti.UserId = check.UserId;
                    noti.ActivityId = check.ActivityId;
                    noti.Status = true;
                    noti.NotificationId = "Noti" + Guid.NewGuid().ToString().Substring(0, 6);
                    await this._context.Notification.AddAsync(noti);
                    await this._context.SaveChangesAsync();
                }
               
                // noti cho những người khác
                var flj = await this._context.FollowJoinAvtivity.Where(x=>x.ActivityId.Equals(_like.ActivityId) && x.IsFollow == true).ToListAsync();
                if (flj != null)
                {
                    foreach(var x in flj)
                    {
                        if (!x.UserId.Equals(check.UserId))
                        {
                            noti = new Notification();
                            if (userlike.FullName.Equals("none"))
                            {
                                noti.Title = userlike.Username + " đã thích chiến dịch " + check.Title;
                            }
                            else
                            {
                                noti.Title = userlike.FullName + " đã thích chiến dịch " + check.Title;
                            }
                            noti.NotificationContent = "Đã có tình nguyện viên thích chiến dịch mà bạn đã theo dõi hoặc tham gia";
                            noti.Datetime = DateTime.Now;
                            noti.UserId = x.UserId;
                            noti.ActivityId = check.ActivityId;
                            noti.Status = true;
                            noti.NotificationId = "Noti" + Guid.NewGuid().ToString().Substring(0, 6);
                            await this._context.Notification.AddAsync(noti);
                            await this._context.SaveChangesAsync();
                        }
                    }
                }
                return true;
            }
            catch
            {
                throw new Exception();
            }
        }

        public async Task<bool> SimpleUnLike(LikeDTO likeInfo)
        {
            try
            {
                var db = await this._context.Like.Where(like => like.ActivityId.Equals(likeInfo.ActivityId) && like.UserId.Equals(likeInfo.UserId)).FirstOrDefaultAsync();

                if (db != null)
                {
                    this._context.Like.Remove(db);
                    this._context.SaveChanges();
                }
                var check = await this._context.Activity.Where(x => x.ActivityId.Equals(likeInfo.ActivityId)).FirstOrDefaultAsync();
                if (check != null)
                {
                    if(check.NumberLike > 0)
                    {
                        check.NumberLike -= 1;
                    }     
                }
                this._context.Activity.Update(check);
                return await this._context.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }

        public Task<Like> UpdateLike(LikeDTO upLike)
        {
            throw new NotImplementedException();
        }
    }
}

