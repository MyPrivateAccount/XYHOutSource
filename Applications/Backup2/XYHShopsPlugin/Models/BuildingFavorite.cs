using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHShopsPlugin.Models
{
    /// <summary>
    /// 楼盘收藏
    /// </summary>
    public class BuildingFavorite : TraceUpdateBase
    {
        /// <summary>
        /// 收藏ID
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 楼盘Id
        /// </summary>
        [MaxLength(127)]
        public string BuildingId { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        [MaxLength(127)]
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
        /// 真是姓名
        /// </summary>
        [NotMapped]
        public string UserNikeName { get; set; }

        /// <summary>
        /// 楼盘信息
        /// </summary>
        [NotMapped]
        public Buildings Buildings { get; set; }
    }

    /// <summary>
    /// 收藏条件
    /// </summary>
    public class PageCondition
    {
        public string KeyWord { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
