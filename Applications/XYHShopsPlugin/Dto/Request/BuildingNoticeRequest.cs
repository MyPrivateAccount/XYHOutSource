using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingNoticeRequest
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [StringLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 楼盘Id
        /// </summary>
        [StringLength(127)]
        public string BuildingId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [StringLength(255)]
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        [StringLength(1024)]
        public string Ext1 { get; set; }
        [StringLength(1024)]
        public string Ext2 { get; set; }
        [StringLength(1024)]
        public string Icon { get; set; }
    }
}
