﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SVCW.Models
{
    public partial class Media
    {
        [Key]
        [Column("mediaId")]
        [StringLength(10)]
        public string MediaId { get; set; }
        [Required]
        [Column("linkMedia")]
        public string LinkMedia { get; set; }
        [Required]
        [Column("type")]
        [StringLength(100)]
        public string Type { get; set; }
        [Column("activityId")]
        [StringLength(10)]
        public string ActivityId { get; set; }
        [Column("processId")]
        [StringLength(10)]
        public string ProcessId { get; set; }
        [Column("activityResultId")]
        [StringLength(10)]
        public string ActivityResultId { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("Media")]
        public virtual Activity Activity { get; set; }
        [ForeignKey("ActivityResultId")]
        [InverseProperty("Media")]
        public virtual ActivityResult ActivityResult { get; set; }
        [ForeignKey("ProcessId")]
        [InverseProperty("Media")]
        public virtual Process Process { get; set; }
    }
}