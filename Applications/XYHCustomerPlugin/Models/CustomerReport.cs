using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客户报备
    /// </summary>
    public class CustomerReport : TraceUpdateBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 业务员Id
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        [MaxLength(127)]
        public string CustomerId { get; set; }

        /// <summary>
        /// 报备状态
        /// </summary>
        [MaxLength(255)]
        public ReportStatus ReportStatus { get; set; }

        /// <summary>
        /// 报备时间
        /// </summary>
        public DateTime? ReportTime { get; set; }

        /// <summary>
        /// 报备楼盘
        /// </summary>
        [MaxLength(127)]
        public string BuildingId { get; set; }

        /// <summary>
        /// 报备商铺
        /// </summary>
        [MaxLength(127)]
        public string ShopsId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        [NotMapped]
        public CustomerInfo CustomerInfo { get; set; }

        /// <summary>
        /// 楼栋名称
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// 商铺名称
        /// </summary>
        public string ShopsName { get; set; }
    }
    /// <summary>
    /// 报备状态
    /// </summary>
    public enum ReportStatus
    {
        /// <summary>
        /// 已提交
        /// </summary>
        Submit = 0,

        /// <summary>
        /// 已确认（驻场确认）
        /// </summary>
        Confirm = 1,

        /// <summary>
        /// 已报备（向开发商报备）
        /// </summary>
        Report = 2,

        /// <summary>
        /// 已带看（业务员）
        /// </summary>
        BeltLook = 3,

        /// <summary>
        /// 已成交
        /// </summary>
        EndDeal = 4,

        /// <summary>
        /// 超期失效
        /// </summary>
        OverTimeLapse = 5,

        /// <summary>
        /// 手动失效
        /// </summary>
        ManualLapse = 6,
    }

}
