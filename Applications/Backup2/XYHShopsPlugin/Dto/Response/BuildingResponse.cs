using ApplicationCore.Dto;
using System;
using System.Collections.Generic;
using System.Text;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Dto
{
    public class BuildingResponse : TraceUpdateBase
    {
        public string Id { get; set; }
        public BuildingBaseInfoResponse BasicInfo { get; set; }
        public BuildingRuleInfoResponse RuleInfo { get; set; }
        public BuildingFacilitiesInfoResponse FacilitiesInfo { get; set; }
        public BuildingShopInfoResponse ShopInfo { get; set; }
        public string ResidentUser1 { get; set; }
        public string ResidentUser2 { get; set; }
        public string ResidentUser3 { get; set; }
        public string ResidentUser4 { get; set; }

        public string CommissionPlan { get; set; }

        public string Summary { get; set; }

        public string OrganizationId { get; set; }
        public ExamineStatusEnum ExamineStatus { get; set; }


        public DateTime CreateTime { get; set; }

        public string Icon { get; set; }
        public UserInfo ResidentUserManager { get; set; }
        public UserInfo ResidentUser1Info { get; set; }
        public UserInfo ResidentUser2Info { get; set; }
        public UserInfo ResidentUser3Info { get; set; }
        public UserInfo ResidentUser4Info { get; set; }


        public List<FileItemResponse> FileList { get; set; }

        public List<AttachmentResponse> AttachmentList { get; set; }

        public List<BuildingNoCreateResponse> BuildingNoInfos { get; set; }

        public List<UpdateRecordResponse> UpdateRecordResponses { get; set; }
        //来源
        public string Source { get; set; }

        //来源ID
        public string SourceId { get; set; }

        public bool IsFavorite { get; set; }

        public bool IsDeleted { get; set; }

    }






}
