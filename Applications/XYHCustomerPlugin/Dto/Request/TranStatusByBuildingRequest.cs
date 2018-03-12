using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Request
{
    public class TranStatusByBuildingRequest
    {
        /// <summary>
        /// 楼盘Id
        /// </summary>
        public string BuildingId { get; set; }

        /// <summary>
        /// 报备状态 
        /// </summary>
        public List<TransactionsStatus> TransactionsStatus { get; set; }

        /// <summary>
        /// 关键字
        /// </summary>
        public string KeyWord { get; set; }
    }
}
