using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    public class TransferringRequest
    {
        /// <summary>
        /// 原所属人
        /// </summary>
        public string SourceUserId { get; set; }
        public string SourceUserName { get; set; }
        /// <summary>
        /// 原所属部门
        /// </summary>
        public string SourceDepartmentId { get; set; }
        public string SourceDepartmentName { get; set; }
        /// <summary>
        /// 移交人
        /// </summary>
        public string TerUserId { get; set; }
        public string TerUserName { get; set; }
        /// <summary>
        /// 移交人部门
        /// </summary>
        public string TerDepartmentId { get; set; }
        public string TerDepartmentName { get; set; }
        /// <summary>
        /// 调离客户Id
        /// </summary>
        public List<CostomerIdNames> Customers { get; set; }
        /// <summary>
        /// 是否保留以前的信息
        /// </summary>
        public bool IsKeep { get; set; }
    }
    public class CostomerIdNames
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
