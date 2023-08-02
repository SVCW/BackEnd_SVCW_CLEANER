﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SVCW.Models
{
    public partial class Donation
    {
        [Key]
        [Column("donationId")]
        [StringLength(10)]
        public string DonationId { get; set; }
        [Required]
        [Column("title")]
        public string Title { get; set; }
        [Column("amount", TypeName = "decimal(18, 0)")]
        public decimal Amount { get; set; }
        [Column("datetime", TypeName = "datetime")]
        public DateTime Datetime { get; set; }
        [Column("email")]
        [StringLength(100)]
        public string Email { get; set; }
        [Column("phone")]
        [StringLength(15)]
        public string Phone { get; set; }
        [Column("name")]
        [StringLength(100)]
        public string Name { get; set; }
        [Column("isAnonymous")]
        public bool? IsAnonymous { get; set; }
        [Required]
        [Column("status")]
        [StringLength(50)]
        public string Status { get; set; }
        [Required]
        [Column("activityId")]
        [StringLength(10)]
        public string ActivityId { get; set; }
        [Column("taxVNPay")]
        public string TaxVnpay { get; set; }
        [Column("userId")]
        [StringLength(10)]
        public string UserId { get; set; }
        [Column("payDate", TypeName = "datetime")]
        public DateTime? PayDate { get; set; }

        [ForeignKey("ActivityId")]
        [InverseProperty("Donation")]
        public virtual Activity Activity { get; set; }
        [ForeignKey("UserId")]
        [InverseProperty("Donation")]
        public virtual User User { get; set; }
    }
}