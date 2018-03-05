using System;
using System.Collections.Generic;
using System.Text;
using XYHShopsPlugin.Dto.Response;

namespace XYHShopsPlugin.Dto
{
    public class ShopInfoResponse
    {
        public string Id { get; set; }
        public string BuildingId { get; set; }
        public string OrganizationId { get; set; }
        public ShopBaseInfoResponse BasicInfo { get; set; }
        public ShopFacilitiesResponse FacilitiesInfo { get; set; }
        public ShopLeaseInfoResponse LeaseInfo { get; set; }
        public string Summary { get; set; }

        public int? ExamineStatus { get; set; }
        public BuildingResponse Buildings { get; set; }

        public BuildingNoCreateResponse BuildingNo { get; set; }
        public string Icon { get; set; }
        public bool IsDeleted { get; set; }
        public List<FileItemResponse> FileList { get; set; }
        public List<AttachmentResponse> AttachmentList { get; set; }

        //来源
        public string Source { get; set; }

        //来源ID
        public string SourceId { get; set; }

        public bool IsFavorite { get; set; }
    }
}
