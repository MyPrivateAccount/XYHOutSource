using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHShopsPlugin.Models
{
    public class BuildingNo : TraceUpdateBase
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 楼盘Id
        /// </summary>
        public string BuildingId { get; set; }

        /// <summary>
        /// 栋数
        /// </summary>
        public string Storied { get; set; }

        /// <summary>
        /// 开盘时间
        /// </summary>
        public DateTime? OpenDate { get; set; }

        /// <summary>
        /// 交房时间
        /// </summary>
        public DateTime? DeliveryDate { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 部门
        /// </summary>
        public string OrganizationId { get; set; }

        public bool IsDeleted { get; set; }
    }
}
