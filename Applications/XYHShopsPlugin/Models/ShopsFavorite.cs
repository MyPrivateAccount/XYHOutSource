using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Dto.Response;

namespace XYHShopsPlugin.Models
{
    public class ShopsFavorite : TraceUpdateBase
    {

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 商铺Id
        /// </summary>
        public string ShopsId { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 收藏时间
        /// </summary>
        public DateTime FavoriteTime { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        [NotMapped]
        public string UserNikeName { get; set; }

        /// <summary>
        /// 商铺信息
        /// </summary>
        [NotMapped]
        public Shops Shops { get; set; }
    }
}
