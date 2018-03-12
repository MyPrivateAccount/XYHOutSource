using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    /// <summary>
    /// 成交信息请求体
    /// </summary>
    public class TransactionsFollowUpRequest
    {
        /// <summary>
        /// 成交Id
        /// </summary>
        public string CustomerTransactionsId { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Contents { get; set; }
    }
}
