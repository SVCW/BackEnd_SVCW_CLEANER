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
                var activity = await this._context.Activity.Where(x => x.ActivityId.Equals(comment.ActivityId)).FirstOrDefaultAsync();
                if (activity.Status.Equals("Pending"))
                {
                    throw new Exception("Chiến dịch chưa được duyệt");
                }
                if (activity.Status.Equals("Reject"))
                {
                    throw new Exception("Chiến dịch bị từ chối");
                }
                if (activity.Status.Equals("Quit"))
                {
                    throw new Exception("Chiến dịch đã bị chủ sở hữu hủy sớm nên bạn không thể thực hiện hành động này");
                }
                var cmt = new Comment();
                cmt.Status = true;
                cmt.UserId = comment.UserId;
                cmt.CommentContent = comment.CommentContent;
                cmt.CommentId = "CMT" + Guid.NewGuid().ToString().Substring(0, 7);
                cmt.Datetime = DateTime.Now;
                cmt.ActivityId = comment.ActivityId;

                await this._context.Comment.AddAsync(cmt);
                await this._context.SaveChangesAsync();

                var usercmt = await this._context.User.Where(x=>x.UserId.Equals(cmt.UserId)).FirstOrDefaultAsync();
                var check = await this._context.Activity.Where(x=>x.ActivityId.Equals(cmt.ActivityId)).FirstOrDefaultAsync();
                var flj = await this._context.FollowJoinAvtivity.Where(x=>x.ActivityId.Equals(cmt.ActivityId)).ToListAsync();
                var noti = new Notification();

                if(!check.UserId.Equals(cmt.UserId))
                {
                    // noti cho chủ sở hữu
                    noti = new Notification();
                    if (usercmt.FullName.Equals("none"))
                    {
                        noti.Title = usercmt.Username + " đã bình luận chiến dịch của bạn";
                    }
                    else
                    {
                        noti.Title = usercmt.FullName + " đã bình luận chiến dịch của bạn";
                    }
                    noti.NotificationContent = "Đã có tình nguyện viên bình luận chiến dịch của bạn";
                    noti.Datetime = DateTime.Now;
                    noti.UserId = check.UserId;
                    noti.ActivityId = check.ActivityId;
                    noti.Status = true;
                    noti.NotificationId = "Noti" + Guid.NewGuid().ToString().Substring(0, 6);
                    await this._context.Notification.AddAsync(noti);
                    await this._context.SaveChangesAsync();
                }

                if (flj != null)
                {
                    foreach (var x in flj)
                    {
                        noti = new Notification();
                        if (usercmt.FullName.Equals("none"))
                        {
                            noti.Title = usercmt.Username + " đã bình luận chiến dịch " + check.Title;
                        }
                        else
                        {
                            noti.Title = usercmt.FullName + " đã bình luận chiến dịch " + check.Title;
                        }
                        noti.NotificationContent = "Đã có tình nguyện viên bình luận chiến dịch mà bạn đã theo dõi hoặc tham gia";
                        noti.Datetime = DateTime.Now;
                        noti.UserId = x.UserId;
                        noti.ActivityId = check.ActivityId;
                        noti.Status = true;
                        noti.NotificationId = "Noti" + Guid.NewGuid().ToString().Substring(0, 6);
                        await this._context.Notification.AddAsync(noti);
                        await this._context.SaveChangesAsync();
                    }
                }
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
                    .Where(x => x.ReplyId == null)
                    .Include(x=>x.User)
                    .Include(x=>x.InverseReply)
                        .ThenInclude(x=>x.User)
                    .OrderByDescending(x=>x.Datetime)
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
                var db = await this._context.Comment.Where(x => x.Status && x.ReplyId == null)
                    .Include(x => x.User)
                    .Include(x => x.InverseReply)
                        .ThenInclude(x => x.User)
                    .OrderByDescending(x => x.Datetime)
                    .ToListAsync();
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
                var activity = await this._context.Activity.Where(x => x.ActivityId.Equals(reply.ActivityId)).FirstOrDefaultAsync();
                if (activity.Status.Equals("Pending"))
                {
                    throw new Exception("Chiến dịch chưa được duyệt");
                }
                if (activity.Status.Equals("Reject"))
                {
                    throw new Exception("Chiến dịch bị từ chối");
                }
                if (activity.Status.Equals("Quit"))
                {
                    throw new Exception("Chiến dịch đã bị chủ sở hữu hủy sớm nên bạn không thể thực hiện hành động này");
                }
                var rep = new Comment();

                rep.Status = true;
                rep.ActivityId = reply.ActivityId;
                rep.CommentContent = reply.CommentContent;
                rep.CommentId = "CMT" + Guid.NewGuid().ToString().Substring(0, 7);
                rep.Datetime = DateTime.Now;
                rep.ReplyId = reply.CommentIdReply;
                rep.UserId = reply.UserId;

                await this._context.Comment.AddAsync(rep);
                await this._context.SaveChangesAsync();

                var cmt = await this._context.Comment.Where(x=>x.CommentId.Equals(rep.ReplyId)).FirstOrDefaultAsync();
                var usercmt = await this._context.User.Where(x => x.UserId.Equals(rep.UserId)).FirstOrDefaultAsync();
                var check = await this._context.Activity.Where(x => x.ActivityId.Equals(rep.ActivityId)).FirstOrDefaultAsync();
                var noti = new Notification();

                if (!check.UserId.Equals(rep.UserId))
                {
                    // noti cho chủ sở hữu
                    noti = new Notification();
                    if (usercmt.FullName.Equals("none"))
                    {
                        noti.Title = usercmt.Username + " đã bình luận chiến dịch của bạn";
                    }
                    else
                    {
                        noti.Title = usercmt.FullName + " đã bình luận chiến dịch của bạn";
                    }
                    noti.NotificationContent = "Đã có tình nguyện viên bình luận chiến dịch của bạn";
                    noti.Datetime = DateTime.Now;
                    noti.UserId = check.UserId;
                    noti.ActivityId = check.ActivityId;
                    noti.Status = true;
                    noti.NotificationId = "Noti" + Guid.NewGuid().ToString().Substring(0, 6);
                    await this._context.Notification.AddAsync(noti);
                    await this._context.SaveChangesAsync();
                }
                noti = new Notification();
                if (usercmt.FullName.Equals("none"))
                {
                    noti.Title = usercmt.Username + " đã trả lời bình luận của bạn";
                }
                else
                {
                    noti.Title = usercmt.FullName + " đã trả lời bình luận của bạn";
                }
                noti.NotificationContent = "Đã có tình nguyện viên trả lời bình luận chiến dịch của bạn";
                noti.Datetime = DateTime.Now;
                noti.UserId = cmt.UserId;
                noti.ActivityId = check.ActivityId;
                noti.Status = true;
                noti.NotificationId = "Noti" + Guid.NewGuid().ToString().Substring(0, 6);
                await this._context.Notification.AddAsync(noti);
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
                    this._context.Comment.Update(currentComment);
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

