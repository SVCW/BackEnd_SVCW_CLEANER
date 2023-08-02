﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SVCW.Models
{
    public partial class Report
    {
        [Key]
        [Column("reportId")]
        [StringLength(10)]
        public string ReportId { get; set; }
        [Required]
        [Column("title")]
        public string Title { get; set; }
        [Required]
        [Column("reason")]
        public string Reason { get; set; }
        [Required]
        [Column("reportTypeId")]
        [StringLength(10)]
        public string ReportTypeId { get; set; }
        [Required]
        [Column("description")]
        public string Description { get; set; }
        [Column("status")]
        public bool Status { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(10)]
        public string UserId { get; set; }
        [Required]
        [Column("activityId")]
        [StringLength(10)]
        public string ActivityId { get; set; }
        [Column("datetime", TypeName = "datetime")]
        public DateTime Datetime { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("Report")]
        public virtual Activity Activity { get; set; }
        [ForeignKey("ReportTypeId")]
        [InverseProperty("Report")]
        public virtual ReportType ReportType { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Report")]
        public virtual User User { get; set; }
    }
}