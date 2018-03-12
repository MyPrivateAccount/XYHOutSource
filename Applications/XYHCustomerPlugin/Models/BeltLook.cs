using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 带看信息
    /// </summary>
    public class BeltLook : TraceUpdateBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 带看结果，内部字典数据（0未处理1有意向2无意向3未看）
        /// </summary>
        public int LookResult { get; set; }
        /// <summary>
        /// 带看状态，内部字典数据（0带看中1已完成2未反馈3异常作废）
        /// </summary>
        public BeltLookState BeltState { get; set; }
        /// <summary>
        /// 带看状态信息（0/1）未确定，暂不使用
        /// </summary>
        public string BeltStateInfo { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        [MaxLength(127)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 带看部门Id
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 带看用户Id
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }
        /// <summary>
        /// 带看时间
        /// </summary>
        public DateTime? BeltTime { get; set; }
        /// <summary>
        /// 带看信息（首看，复看，二看）前台业务员确认
        /// </summary>
        [MaxLength(50)]
        public string BeltInfo { get; set; }
        /// <summary>
        /// 外出时间
        /// </summary>
        public DateTime? OutTime { get; set; }
        /// <summary>
        /// 反馈时间
        /// </summary>
        public DateTime? FeedbackTime { get; set; }
        /// <summary>
        /// 陪看部门Id
        /// </summary>
        [MaxLength(127)]
        public string AccompanyDepartmentId { get; set; }
        /// <summary>
        /// 陪看用户Id
        /// </summary>
        [MaxLength(127)]
        public string AccompanySeeId { get; set; }
        /// <summary>
        /// 返回时间
        /// </summary>
        public DateTime? EstimatebackTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 带看房源，楼盘Id
        /// </summary>
        [MaxLength(127)]
        public string BeltHouse { get; set; }
        /// <summary>
        /// 证件类型，内部字典数据，1身份证2军官证3 驾驶证4 护照5 其他 
        /// </summary>
        public int Certificates { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        [MaxLength(50)]
        public string CertificatesNo { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 完成带看次数
        /// </summary>
        public int CompleteBeltNum { get; set; }
        /// <summary>
        /// 房源类型，内部字典数据（1住宅求租2住宅求购 3商业求租4商业求购）
        /// </summary>
        public NeedHouseType? HouseTypeId { get; set; }
        /// <summary>
        /// 未确定，暂不使用
        /// </summary>
        public int LookResultCount { get; set; }
        /// <summary>
        /// 驻场用户
        /// </summary>
        public string InSiteUser { get; set; }
        /// <summary>
        /// 附件数量，BeltLookFiles相关
        /// </summary>
        public int ImageCount { get; set; }
    }


    public enum BeltLookState
    {
        WaitLook = 1,
        WaitDeal = 2,
        EndDeal = 4,
        Cancel = 8

    }
}
