using SVCW.DTOs.Likes;
using SVCW.Models;

namespace SVCW.Interfaces
{
	public interface ILike
	{
		Task<List<Like>> GetActivityLikes(string activityId);
		Task<bool> SimpleLike(LikeDTO likeInfo);
		Task<bool> SimpleUnLike(LikeDTO likeInfo);
		Task<Like> UpdateLike(LikeDTO upLike);
		Task<bool> DeleteLikeByIds(string? userId, string? activityId);
		Task<List<Like>> InsertLike(List<LikeDTO> listLikes);
	}
}

