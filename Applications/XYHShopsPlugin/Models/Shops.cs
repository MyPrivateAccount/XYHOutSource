using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHShopsPlugin.Models
{
    public class Shops : TraceUpdateBase
    {
        [MaxLength(127)]
        public string Id { get; set; }

        [MaxLength(127)]
        public string BuildingId { get; set; }

        [Required]
        [MaxLength(127)]
        public string OrganizationId { get; set; }
        [MaxLength(5000)]
        public string Summary { get; set; }

        public int? ExamineStatus { get; set; }

        [MaxLength(512)]
        public string Icon { get; set; }
        public bool IsDeleted { get; set; }

        [NotMapped]
        public ShopBaseInfo ShopBaseInfo { get; set; }

        [NotMapped]
        public ShopFacilities ShopFacilities { get; set; }

        [NotMapped]
        public ShopLeaseInfo ShopLeaseInfo { get; set; }

        [NotMapped]
        public Buildings Buildings { get; set; }

        [NotMapped]
        public BuildingNo BuildingNo { get; set; }

        [NotMapped]
        public IEnumerable<FileInfo> ShopsFileInfos { get; set; }
        //来源
        [MaxLength(32)]
        public string Source { get; set; }

        //来源ID
        [MaxLength(64)]
        public string SourceId { get; set; }
    }
}
