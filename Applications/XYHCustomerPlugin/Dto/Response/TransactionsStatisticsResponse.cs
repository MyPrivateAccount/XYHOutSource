using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Response
{
    public class TransactionsStatisticsResponse
    {
        /// <summary>
        /// 已提交行数
        /// </summary>
        public int SubmitCount { get; set; }

        /// <summary>
        /// 已确认行数
        /// </summary>
        public int ConfirmCount { get; set; }

        /// <summary>
        /// 已报备行数
        /// </summary>
        public int ReportCount { get; set; }

        /// <summary>
        /// 已带看行数
        /// </summary>
        public int BeltLookCount { get; set; }

        /// <summary>
        /// 已成交行数
        /// </summary>
        public int EndDealCount { get; set; }

        /// <summary>
        /// 超期失效行数
        /// </summary>
        public int OverTimeLapseCount { get; set; }

        /// <summary>
        /// 手动失效行数
        /// </summary>
        public int ManualLapseCount { get; set; }
    }
}
