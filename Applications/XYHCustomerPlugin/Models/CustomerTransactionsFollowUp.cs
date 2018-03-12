using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客户成交跟进
    /// </summary>
    public class CustomerTransactionsFollowUp : TraceUpdateBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public string Id { get; set; }
        /// <summary>
        /// 成交Id
        /// </summary>
        public string CustomerTransactionsId { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Contents { get; set; }
        /// <summary>
        /// 跟进用户Id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 时间的备注信息
        /// </summary>
        public DateTime? MarkTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public TransactionsStatus TransactionsStatus { get; set; }
        
    }
}
