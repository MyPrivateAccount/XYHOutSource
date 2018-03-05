using ApplicationCore.Dto;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Dto
{
    public class BuildingRequest : TraceUpdateBase
    {
        [Required]
        [StringLength(127)]
        public string Id { get; set; }
        //   public string BaseInfo { get; set; }
        //    public string FacilitiesInfo { get; set; }
        //     public string ShopInfo { get; set; }
        [StringLength(127)]
        public string ResidentUser1 { get; set; }
        [StringLength(127)]
        public string ResidentUser2 { get; set; }
        [StringLength(127)]
        public string ResidentUser3 { get; set; }
        [StringLength(127)]
        public string ResidentUser4 { get; set; }
        public string CommissionPlan { get; set; }
        public string Summary { get; set; }
        // [Required]
        [StringLength(127)]
        public string OrganizationId { get; set; }

        public ExamineStatusEnum ExamineStatus { get; set; }
        [StringLength(512)]
        public string Icon { get; set; }
        //来源
        public string Source { get; set; }

        //来源ID
        public string SourceId { get; set; }
        //     public string Ext1 { get; set; }
        //     public string Ext2 { get; set; }


    }
}
