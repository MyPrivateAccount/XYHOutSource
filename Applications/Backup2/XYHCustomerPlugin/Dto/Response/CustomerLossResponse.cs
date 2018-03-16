using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerLossResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 失效类型（已购、失效、暂停、外泄）
        /// </summary>
        public LossType LossTypeId { get; set; }
        /// <summary>
        /// 失效备注
        /// </summary>
        public string LossRemark { get; set; }
        /// <summary>
        /// 失效部门
        /// </summary>
        public string LossDepartmentId { get; set; }
        /// <summary>
        /// 失效用户
        /// </summary>
        public string LossUserId { get; set; }
        /// <summary>
        /// 失效用户姓名
        /// </summary>
        public string LossUserTrueName { get; set; }
        /// <summary>
        /// 失效时间
        /// </summary>
        public DateTime LossTime { get; set; }
    }
}
