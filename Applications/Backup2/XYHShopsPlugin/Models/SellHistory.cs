using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Models
{
    public class SellHistory : TraceUpdateBase
    {
        /// <summary>
        /// 销售历史Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 楼盘Id
        /// </summary>
        public string BuildingId { get; set; }

        /// <summary>
        /// 商铺Id
        /// </summary>
        public string ShopsId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 销售状态
        /// </summary>
        public string SaleStatus { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public DateTime? LockTime { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }
    }
}
