using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto.Request
{
    public class ShopsFavoriteRequest
    {
        /// <summary>
        /// 收藏ID
        /// </summary>
        [StringLength(127, ErrorMessage = "Id长度为127")]
        public string Id { get; set; }

        /// <summary>
        /// 楼盘Id
        /// </summary>
        [StringLength(127, ErrorMessage = "ShopsId长度为127")]
        public string ShopsId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [StringLength(127, ErrorMessage = "UserId长度为127")]
        public string UserId { get; set; }

        /// <summary>
        /// 收藏时间
        /// </summary>
        public DateTime FavoriteTime { get; set; }
    }
}
