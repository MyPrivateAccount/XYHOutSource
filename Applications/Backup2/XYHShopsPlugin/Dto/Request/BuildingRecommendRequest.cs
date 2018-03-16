using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto.Request
{
    public class BuildingRecommendRequest
    {
        [StringLength(127, ErrorMessage = "Id最大长度为127")]
        public string Id { get; set; }

        [StringLength(127, ErrorMessage = "BuildingId最大长度为127")]
        public string BuildingId { get; set; }

        [StringLength(127, ErrorMessage = "RecommendUserId最大长度为127")]
        public string RecommendUserId { get; set; }

        public DateTime RecommendTime { get; set; }

        public int RecommendDays { get; set; }

        public int Order { get; set; }

        /// <summary>
        /// 是否是大区推荐
        /// </summary>
        public bool IsRegion { get; set; }
    }
}
