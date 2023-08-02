using System;
using System.ComponentModel.DataAnnotations;

namespace SVCW.DTOs.Comments
{
    public class ReplyCommentDTO
    {
        public string UserId { get; set; }
        public string ActivityId { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string? CommentContent { get; set; }
        public bool? status { get; set; }
        public string? CommentIdReply { get; set; }
    }
}

