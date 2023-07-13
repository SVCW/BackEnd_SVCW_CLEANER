using SVCW.DTOs.Comments;
using SVCW.Models;

namespace SVCW.Interfaces
{
	public interface IComment
	{
        Task<Comment> comment(CommentDTO comment);
        Task<Comment> ReplyComment(ReplyCommentDTO reply);
        Task<List<Comment>> GetComments();
        Task<List<Comment>> GetCommentUser();
        Task<Comment> UpdateComment(UpdateCommentDTO comment);
        Task<bool> DeleteComment(string id);
    }
}

