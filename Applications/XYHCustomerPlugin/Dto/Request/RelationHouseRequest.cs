using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    public class RelationHouseRequest
    {

        /// <summary>
        /// 房源Id
        /// </summary>
        public string HousingResourcesId { get; set; }
        /// <summary>
        /// 房源名称
        /// </summary>
        [StringLength(255, ErrorMessage = "PropertyName最大字符为255")]
        public string PropertyName { get; set; }
        /// <summary>
        /// 房间编号（后期可以加商铺编号）
        /// </summary>
        [StringLength(127, ErrorMessage = "RoomNo最大字符为127")]
        public string RoomNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1024, ErrorMessage = "Remark最大字符为1024")]
        public string Remark { get; set; }

        /// <summary>
        /// 商铺编号
        /// </summary>
        [StringLength(255, ErrorMessage = "Remark最大字符为255")]
        public string ShopNumber { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        [StringLength(255, ErrorMessage = "Remark最大字符为255")]
        public string AreaFullName { get; set; }

        public string ImageUrl { get; set; }

    }
}
