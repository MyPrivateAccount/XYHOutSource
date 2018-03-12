using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Request
{
    /// <summary>
    /// 报备请求体
    /// </summary>
    public class CustomerReportRequest
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 客户Id
        /// </summary>
        [StringLength(127, ErrorMessage = "CustomerId最大长度为127")]
        public string CustomerId { get; set; }

        /// <summary>
        /// 客户信息请求体
        /// </summary>
        public CustomerInfoCreateRequest CustomerInfoCreateRequest { get; set; }

        /// <summary>
        /// 报备状态
        /// </summary>
        public ReportStatus ReportStatus { get; set; }

        /// <summary>
        /// 报备时间
        /// </summary>
        public DateTime? ReportTime { get; set; }

        /// <summary>
        /// 报备楼盘
        /// </summary>
        [StringLength(127, ErrorMessage = "BuildingId最大长度为127")]
        public string BuildingId { get; set; }

        /// <summary>
        /// 报备商铺
        /// </summary>
        [StringLength(127, ErrorMessage = "ShopsId最大长度为127")]
        public string ShopsId { get; set; }

        /// <summary>
        /// 楼栋名称
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// 商铺名称
        /// </summary>
        public string ShopsName { get; set; }

    }
}
