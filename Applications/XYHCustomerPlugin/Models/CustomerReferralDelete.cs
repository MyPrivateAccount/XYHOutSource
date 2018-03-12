using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客源转移删除（暂未使用）
    /// </summary>
    public class CustomerReferralDelete
    {
        public string Id { get; set; }
        public string CustomerId { get; set; }
        public string CustomerNo { get; set; }
        public string DepartmentId { get; set; }
        public string UserId { get; set; }
        public string ReferralDepartmentId { get; set; }
        public string ReferralUserId { get; set; }
        public string ReceiveDepartmentId { get; set; }
        public string ReceiveUserId { get; set; }
        public int ReferralType { get; set; }
        public DateTime? ClientRecordTime { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        public string CompanyId { get; set; }


    }
}
