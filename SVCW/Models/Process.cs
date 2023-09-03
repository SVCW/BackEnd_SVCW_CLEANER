﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SVCW.Models
{
    public partial class Process
    {
        public Process()
        {
            Media = new HashSet<Media>();
        }

        [Key]
        [Column("processId")]
        [StringLength(10)]
        public string ProcessId { get; set; }
        [Required]
        [Column("processTitle")]
        public string ProcessTitle { get; set; }
        [Required]
        public string Description { get; set; }
        [Column("datetime", TypeName = "datetime")]
        public DateTime Datetime { get; set; }
        [Column("startDate", TypeName = "datetime")]
        public DateTime? StartDate { get; set; }
        [Column("endDate", TypeName = "datetime")]
        public DateTime? EndDate { get; set; }
        [Column("status")]
        public bool Status { get; set; }
        [Column("activityId")]
        [StringLength(10)]
        public string ActivityId { get; set; }
        [Column("processTypeId")]
        [StringLength(10)]
        public string ProcessTypeId { get; set; }
        [Column("activityResultId")]
        [StringLength(10)]
        public string ActivityResultId { get; set; }
        [Column("processNo")]
        public int? ProcessNo { get; set; }
        [Column("isKeyProcess")]
        public bool? IsKeyProcess { get; set; }
        [Column("location")]
        public string Location { get; set; }
        [Column("targetParticipant")]
        public int? TargetParticipant { get; set; }
        [Column("realParticipant")]
        public int? RealParticipant { get; set; }
        [Column("isDonateProcess")]
        public bool? IsDonateProcess { get; set; }
        [Column("isParticipant")]
        public bool? IsParticipant { get; set; }
        [Column("realDonation", TypeName = "decimal(18, 0)")]
        public decimal? RealDonation { get; set; }
        [Column("targetDonation", TypeName = "decimal(18, 0)")]
        public decimal? TargetDonation { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("Process")]
        public virtual Activity Activity { get; set; }
        [ForeignKey("ActivityResultId")]
        [InverseProperty("Process")]
        public virtual ActivityResult ActivityResult { get; set; }
        [ForeignKey("ProcessTypeId")]
        [InverseProperty("Process")]
        public virtual ProcessType ProcessType { get; set; }
        [InverseProperty("Process")]
        public virtual ICollection<Media> Media { get; set; }
    }
}