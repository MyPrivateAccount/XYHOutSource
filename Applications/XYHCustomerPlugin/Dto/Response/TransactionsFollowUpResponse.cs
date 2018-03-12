using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Response
{
    /// <summary>
    /// 成交信息返回体
    /// </summary>
    public class TransactionsFollowUpResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 成交Id
        /// </summary>
        public DateTime CreateTime { get; set; }
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
        /// 备注时间
        /// </summary>
        public DateTime? MarkTime { get; set; }
        /// <summary>
        /// 跟进状态
        /// </summary>
        public TransactionsStatus TransactionsStatus { get; set; }

    }
}
