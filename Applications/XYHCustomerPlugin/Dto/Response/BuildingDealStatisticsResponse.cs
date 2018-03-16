using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Response
{
    public class BuildingDealStatisticsResponse
    {
        /// <summary>
        /// 成交套数
        /// </summary>
        public int DealCount { get; set; }

        /// <summary>
        /// 佣金金额
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 成交价格
        /// </summary>
        public decimal DealPrice { get; set; }
    }
}
