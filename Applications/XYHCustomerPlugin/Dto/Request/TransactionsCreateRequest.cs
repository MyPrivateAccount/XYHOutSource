using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Request
{
    /// <summary>
    /// 新增报备
    /// </summary>
    public class TransactionsCreateRequest
    {
        //以下是新增信息

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }


        //public CustomerInfoCreateRequest CustomerInfoCreateRequest { get; set; }

        /// <summary>
        /// 客户Id集合
        /// </summary>
        public List<string> CustomerIds { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public TransactionsStatus TransactionsStatus { get; set; }

        /// <summary>
        /// 报备时间
        /// </summary>
        public DateTime ReportTime { get; set; }

        /// <summary>
        /// 报备楼盘
        /// </summary>
        public string BuildingId { get; set; }

        /// <summary>
        /// 报备商铺
        /// </summary>
        public string ShopsId { get; set; }

        /// <summary>
        /// 楼栋名称
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// 商铺名称
        /// </summary>
        public string ShopsName { get; set; }

        /// <summary>
        /// 预计带看时间
        /// </summary>
        public DateTime? ExpectedBeltTime { get; set; }

    }
}
