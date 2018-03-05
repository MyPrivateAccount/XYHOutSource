using ApplicationCore.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XYHShopsPlugin.Dto;

namespace XYHShopsPlugin.Models
{
    public class BuildingRecommend : TraceUpdateBase
    {
        /// <summary>
        /// 楼盘推荐Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 推荐楼盘Id
        /// </summary>
        [MaxLength(127)]
        public string BuildingId { get; set; }

        /// <summary>
        /// 推荐人ID
        /// </summary>
        [MaxLength(127)]
        public string RecommendUserId { get; set; }

        /// <summary>
        /// 推荐时间
        /// </summary>
        public DateTime RecommendTime { get; set; }

        /// <summary>
        /// 推荐周期
        /// </summary>
        public int RecommendDays { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        
        /// <summary>
        /// 是否是大区推荐
        /// </summary>
        public bool IsRegion { get; set; }

        [MaxLength(127)]
        public string MainAreaId { get; set; }

        /// <summary>
        /// 是否过期
        /// </summary>
        public bool IsOutDate { get; set; }

        /// <summary>
        /// 创建人真实姓名
        /// </summary>
        [NotMapped]
        public string UserNikeName { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [NotMapped]
        public string MainAreaName { get; set; }

        /// <summary>
        /// 楼盘信息
        /// </summary>
        [NotMapped]
        public Buildings Buildings { get; set; }
    }
}
