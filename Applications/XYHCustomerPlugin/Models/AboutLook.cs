using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 约看信息
    /// </summary>
    public class AboutLook : TraceUpdateBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        [MaxLength(127)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        [MaxLength(50)]
        public string CustomerNo { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        [MaxLength(50)]
        public string CustomerName { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 约看时间
        /// </summary>
        public DateTime AboutTime { get; set; }
        /// <summary>
        /// 约看状态(待带看，已取消，已带看，已过期)
        /// </summary>
        public AboutLookState AboutState { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(1000)]
        public string Remark { get; set; }
        /// <summary>
        /// 约看房源(多个逗号隔开)
        /// </summary>
        [MaxLength(1000)]
        public string AboutHouse { get; set; }
        /// <summary>
        /// 房源类型（楼盘、商铺、住宅等等）
        /// </summary>
        public NeedHouseType? HouseTypeId { get; set; }

        /// <summary>
        /// 删除状态
        /// </summary>
        public bool IsDeleted { get; set; }
    }

    public enum AboutLookState
    {
        WaitLook = 1,
        WaitDeal = 2,
        EndDeal = 4,
        Cancel = 8

    }
}
