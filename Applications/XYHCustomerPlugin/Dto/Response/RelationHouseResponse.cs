using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Dto.Response
{
    public class RelationHouseResponse
    {
        /// <summary>
        /// 房源Id
        /// </summary>
        public string HousingResourcesId { get; set; }
        /// <summary>
        /// 房源名称
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 房间编号（后期可以加商铺编号）
        /// </summary>
        public string RoomNo { get; set; }
        /// <summary>
        /// 商铺编号
        /// </summary>
        public string ShopNumber { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string AreaFullName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }
        
        public string MainPhone { get; set; }

        public string ImageUrl { get; set; }
    }
}
