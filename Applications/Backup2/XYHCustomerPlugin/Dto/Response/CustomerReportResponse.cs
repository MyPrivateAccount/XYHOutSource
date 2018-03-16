using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Response
{
    public class CustomerReportResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 业务员Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// 报备状态
        /// </summary>
        public ReportStatus ReportStatus { get; set; }

        /// <summary>
        /// 报备时间
        /// </summary>
        public string ReportTime { get; set; }

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

    }
}
