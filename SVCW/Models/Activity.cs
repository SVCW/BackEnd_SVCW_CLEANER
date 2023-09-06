﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SVCW.Models
{
    public partial class Activity
    {
        public Activity()
        {
            ActivityResult = new HashSet<ActivityResult>();
            Comment = new HashSet<Comment>();
            Donation = new HashSet<Donation>();
            FollowJoinAvtivity = new HashSet<FollowJoinAvtivity>();
            InverseReActivityNavigation = new HashSet<Activity>();
            Like = new HashSet<Like>();
            Media = new HashSet<Media>();
            Process = new HashSet<Process>();
            RejectActivity = new HashSet<RejectActivity>();
            Report = new HashSet<Report>();
            BankAccount = new HashSet<BankAccount>();
        }

        [Key]
        [Column("activityId")]
        [StringLength(10)]
        public string ActivityId { get; set; }
        [Required]
        [Column("title")]
        public string Title { get; set; }
        [Required]
        [Column("description")]
        public string Description { get; set; }
        [Column("createAt", TypeName = "datetime")]
        public DateTime CreateAt { get; set; }
        [Column("startDate", TypeName = "datetime")]
        public DateTime StartDate { get; set; }
        [Column("endDate", TypeName = "datetime")]
        public DateTime EndDate { get; set; }
        [Column("location")]
        public string Location { get; set; }
        [Column("numberLike")]
        public int? NumberLike { get; set; }
        [Column("numberJoin")]
        public int? NumberJoin { get; set; }
        [Column("shareLink")]
        public string ShareLink { get; set; }
        [Column("realDonation", TypeName = "decimal(18, 0)")]
        public decimal? RealDonation { get; set; }
        [Column("targetDonation", TypeName = "decimal(18, 0)")]
        public decimal? TargetDonation { get; set; }
        [Required]
        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(10)]
        public string UserId { get; set; }
        [Column("fanpageId")]
        [StringLength(10)]
        public string FanpageId { get; set; }
        [Column("reActivity")]
        [StringLength(10)]
        public string ReActivity { get; set; }

        [ForeignKey("FanpageId")]
        [InverseProperty("Activity")]
        public virtual Fanpage Fanpage { get; set; }
        [ForeignKey("ReActivity")]
        [InverseProperty("InverseReActivityNavigation")]
        public virtual Activity ReActivityNavigation { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Activity")]
        public virtual User User { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<ActivityResult> ActivityResult { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<Comment> Comment { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<Donation> Donation { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<FollowJoinAvtivity> FollowJoinAvtivity { get; set; }
        [InverseProperty("ReActivityNavigation")]
        public virtual ICollection<Activity> InverseReActivityNavigation { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<Like> Like { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<Media> Media { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<Process> Process { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<RejectActivity> RejectActivity { get; set; }
        [InverseProperty("Activity")]
        public virtual ICollection<Report> Report { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("Activity")]
        public virtual ICollection<BankAccount> BankAccount { get; set; }
    }
}