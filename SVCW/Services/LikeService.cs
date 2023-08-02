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
                return await this._context.SaveChangesAsync() > 0;
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
                    check.NumberLike -= 1;
                }
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

