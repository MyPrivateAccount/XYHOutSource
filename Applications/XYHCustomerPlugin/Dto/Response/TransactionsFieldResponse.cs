using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Response
{
    public class TransactionsFieldResponse
    {
        /// <summary>
        /// 已提交行数
        /// </summary>
        public int SubmitCount { get; set; }

        /// <summary>
        /// 今日待报备行数
        /// </summary>
        public int TodayReportCount { get; set; }

        /// <summary>
        /// 待确认ID
        /// </summary>
        public List<string> SubmintIds { get; set; }

        /// <summary>
        /// 待报备ID
        /// </summary>
        public List<TransactionsResponse> TransactionsResponses { get; set; }
    }
}
