using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客源转移
    /// </summary>
    public class CustomerReferral : TraceUpdateBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        [MaxLength(127)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        [MaxLength(127)]
        public string CustomerNo { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 操作用户ID
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }
        /// <summary>
        /// 调出部门ID
        /// </summary>
        [MaxLength(127)]
        public string ReferralDepartmentId { get; set; }
        /// <summary>
        /// 调出用户ID
        /// </summary>
        [MaxLength(127)]
        public string ReferralUserId { get; set; }
        /// <summary>
        /// 接收部门ID
        /// </summary>
        [MaxLength(127)]
        public string ReceiveDepartmentId { get; set; }
        /// <summary>
        /// 接收用户ID
        /// </summary>
        [MaxLength(127)]
        public string ReceiveUserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ReferralType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReferralProportion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ReferralStatus { get; set; }
        /// <summary>
        /// 完成时间
        /// </summary>
        public DateTime? CompleteTime { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
    }
}
