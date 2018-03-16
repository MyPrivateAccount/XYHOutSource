using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Response
{
    public class BeltLookResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int LookResult { get; set; }
        /// <summary>
        /// 带看状态
        /// </summary>
        public BeltLookState BeltState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BeltStateInfo { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 带看时间
        /// </summary>
        public DateTime BeltTime { get; set; }
        /// <summary>
        /// 带看信息（首看，复看）
        /// </summary>
        public string BeltInfo { get; set; }
        /// <summary>
        /// 离开时间
        /// </summary>
        public DateTime OutTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime FeedbackTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccompanyDepartmentId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string AccompanySeeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime EstimatebackTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 带看房源
        /// </summary>
        public string BeltHouse { get; set; }
        /// <summary>
        /// 证件类型
        /// </summary>
        public int Certificates { get; set; }
        /// <summary>
        /// 证件编号
        /// </summary>
        public string CertificatesNo { get; set; }

        /// <summary>
        /// 完成带看次数
        /// </summary>
        public int CompleteBeltNum { get; set; }
        /// <summary>
        /// 房源类型
        /// </summary>
        public NeedHouseType? HouseTypeId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int LookResultCount { get; set; }
        /// <summary>
        /// 驻场用户
        /// </summary>
        public string InSiteUser { get; set; }
        /// <summary>
        /// 图片数量（未用）
        /// </summary>
        public int ImageCount { get; set; }
    }
}
