using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace XYHShopsPlugin.Models
{
    public class Buildings : TraceUpdateBase
    {
        [MaxLength(127)]
        public string Id { get; set; }
        [MaxLength(127)]
        public string ResidentUser1 { get; set; }
        [MaxLength(127)]
        public string ResidentUser2 { get; set; }
        [MaxLength(127)]
        public string ResidentUser3 { get; set; }
        [MaxLength(127)]
        public string ResidentUser4 { get; set; }

        public int? ExamineStatus { get; set; }

        /// <summary>
        /// 佣金方案
        /// </summary>
        public string CommissionPlan { get; set; }

        public string Summary { get; set; }
        [MaxLength(512)]
        public string Icon { get; set; }
        //[MaxLength(127)]
        //public string Attachments { get; set; }
        [Required]
        [MaxLength(127)]
        public string OrganizationId { get; set; }

        public bool IsDeleted { get; set; }

        [NotMapped]
        public BuildingBaseInfo BuildingBaseInfo { get; set; }

        [NotMapped]
        public BuildingRule BuildingRule { get; set; }

        [NotMapped]
        public BuildingFacilities BuildingFacilities { get; set; }

        [NotMapped]
        public BuildingShopInfo BuildingShopInfo { get; set; }

        [NotMapped]
        public SimpleUser ResidentUser1Info { get; set; }
        [NotMapped]
        public SimpleUser ResidentUser2Info { get; set; }
        [NotMapped]
        public SimpleUser ResidentUser3Info { get; set; }
        [NotMapped]
        public SimpleUser ResidentUser4Info { get; set; }

        [NotMapped]
        public Organizations OrganizationInfo { get; set; }

        [NotMapped]
        public IEnumerable<FileInfo> BuildingFileInfos { get; set; }

        [NotMapped]
        public IEnumerable<BuildingNo> BuildingNoInfo { get; set; }

        [NotMapped]
        public IEnumerable<UpdateRecord> UpdateRecords { get; set; }

        //来源
        [MaxLength(32)]
        public string Source { get; set; }

        //来源ID
        [MaxLength(64)]
        public string SourceId { get; set; }
    }
}
