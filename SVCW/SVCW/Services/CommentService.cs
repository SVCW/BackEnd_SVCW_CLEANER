using Microsoft.EntityFrameworkCore;
using SVCW.DTOs.Comments;
using SVCW.Interfaces;
using SVCW.Models;

namespace SVCW.Services
{
    public class CommentService : IComment
    {
        private readonly SVCWContext _context;

        public CommentService(SVCWContext context)
        {
            _context = context;
        }

        public async Task<Comment> comment(CommentDTO comment)
        {
            try
            {
                var cmt = new Comment();
                cmt.Status = true;
                cmt.UserId = comment.UserID;
                cmt.CommentContent = comment.CommentContent;
                cmt.CommentId = "CMT" + Guid.NewGuid().ToString().Substring(0, 7);
                cmt.Datetime = DateTime.Now;
                cmt.UserId = comment.UserID;
                cmt.ActivityId = comment.ActivityId;

                await this._context.Comment.AddAsync(cmt);
                await this._context.SaveChangesAsync();
                return cmt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);

            }

        }

        public async Task<bool> DeleteComment(List<string> id)
        {
            try
            {
                var cmt = await this._context.Comment.Where(x => x.CommentId.Equals(id)).FirstOrDefaultAsync();
                var rep = await this._context.Comment.Where(x => x.ReplyId.Equals(cmt.CommentId)).ToListAsync();
                foreach (var rep2 in rep)
                {
                    rep2.Status = false;
                }
                cmt.Status = false;
                if (await this._context.SaveChangesAsync() > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<bool> DeleteComment(string id)
        {
            try
            {
                var cmt = await this._context.Comment.Where(x => x.CommentId.Equals(id)).FirstOrDefaultAsync();
                var rep = await this._context.Comment.Where(x => x.ReplyId.Equals(cmt.CommentId)).ToListAsync();
                foreach (var rep2 in rep)
                {
                    rep2.Status = false;
                }
                cmt.Status = false;
                if (await this._context.SaveChangesAsync() > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Comment>> GetComments()
        {
            try
            {
                var db = await this._context.Comment
                    .Include(x=>x.User)
                    .ToListAsync();
                return db;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<Comment>> GetCommentUser()
        {
            try
            {
                var db = await this._context.Comment.Where(x => x.Status).ToListAsync();
                return db;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Comment> ReplyComment(ReplyCommentDTO reply)
        {
            try
            {
                var rep = new Comment();

                rep.Status = true;
                rep.ActivityId = reply.ActivityiId;
                rep.CommentContent = reply.CommentContent;
                rep.CommentId = "CMT" + Guid.NewGuid().ToString().Substring(0, 7);
                rep.Datetime = DateTime.Now;
                rep.ReplyId = reply.CommentIdReply;
                rep.UserId = reply.UserId;

                await this._context.Comment.AddAsync(rep);
                await this._context.SaveChangesAsync();
                return rep;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Comment> UpdateComment(UpdateCommentDTO comment)
        {
            try
            {
                var currentComment = await this._context.Comment.Where(x => x.CommentId.Equals(comment.CommentId)).FirstOrDefaultAsync();
                if (currentComment != null)
                {
                    currentComment.CommentContent = comment.CommentContent;
                    await this._context.SaveChangesAsync();
                }
                return currentComment;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

