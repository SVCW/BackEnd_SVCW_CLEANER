﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SVCW.Models
{
    public partial class Comment
    {
        public Comment()
        {
            InverseReply = new HashSet<Comment>();
        }

        [Key]
        [Column("commentId")]
        [StringLength(10)]
        public string CommentId { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(10)]
        public string UserId { get; set; }
        [Required]
        [Column("activityId")]
        [StringLength(10)]
        public string ActivityId { get; set; }
        [Required]
        [Column("commentContent")]
        public string CommentContent { get; set; }
        [Column("datetime", TypeName = "datetime")]
        public DateTime Datetime { get; set; }
        [Column("status")]
        public bool Status { get; set; }
        [Column("replyId")]
        [StringLength(10)]
        public string ReplyId { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("Comment")]
        public virtual Activity Activity { get; set; }
        [ForeignKey("ReplyId")]
        [InverseProperty("InverseReply")]
        public virtual Comment Reply { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Comment")]
        public virtual User User { get; set; }
        [InverseProperty("Reply")]
        public virtual ICollection<Comment> InverseReply { get; set; }
    }
}