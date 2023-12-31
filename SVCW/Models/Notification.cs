﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SVCW.Models
{
    public partial class Notification
    {
        [Key]
        [Column("notificationId")]
        [StringLength(10)]
        public string NotificationId { get; set; }
        [Required]
        [Column("title")]
        public string Title { get; set; }
        [Required]
        [Column("notificationContent")]
        public string NotificationContent { get; set; }
        [Column("datetime", TypeName = "datetime")]
        public DateTime Datetime { get; set; }
        [Column("status")]
        public bool Status { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(10)]
        public string UserId { get; set; }
        [Column("activityId")]
        [StringLength(10)]
        public string ActivityId { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("Notification")]
        public virtual Activity Activity { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Notification")]
        public virtual User User { get; set; }
    }
}