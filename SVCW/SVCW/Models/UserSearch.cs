﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SVCW.Models
{
    public partial class UserSearch
    {
        [Key]
        [Column("userSearchId")]
        [StringLength(10)]
        public string UserSearchId { get; set; }
        [Required]
        [Column("searchContent")]
        public string SearchContent { get; set; }
        [Column("datetime", TypeName = "datetime")]
        public DateTime Datetime { get; set; }
        [Required]
        [Column("userId")]
        [StringLength(10)]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("UserSearch")]
        public virtual User User { get; set; }
    }
}