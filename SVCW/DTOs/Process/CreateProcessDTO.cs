﻿using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SVCW.DTOs.Media;

namespace SVCW.DTOs.Process
{
    public class CreateProcessDTO
    {
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string ProcessTitle { get; set; }
        [RegularExpression(@"^(?!.*(fuck|badword1|badword2|địt|đụ|lồn|cặc|chém|loz|Đm|Duma|Nứng|Ngáo)).*$")]
        public string Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ActivityId { get; set; }
        public string ProcessTypeId { get; set; }
        public bool? IsKeyProcess { get; set; }
        public int? ProcessNo { get; set; }
        public string Location { get; set; }
        public int? TargetParticipant { get; set; }
        public int? RealParticipant { get; set; }
        public bool? IsDonateProcess { get; set; }
        public bool? IsParticipant { get; set; }
        public decimal? RealDonation { get; set; }
        public decimal? TargetDonation { get; set; }
        public List<MediaDTO>? media { get; set; }
    }
}
