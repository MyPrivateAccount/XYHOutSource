using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Dto.Request
{
    public class HumanInfoChangeRequest
    {
        /// <summary>e
        /// 主键Id
        /// </summary>
        [StringLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 人事Id
        /// </summary>
        [StringLength(127)]
        public string HumanId { get; set; }
        /// <summary>
        /// 变动类型
        /// </summary>
        public HumanChangeType ChangeType { get; set; }
        /// <summary>
        /// 变动时间
        /// </summary>
        public DateTime ChangeTime { get; set; }
        /// <summary>
        /// 变动内容
        /// </summary>
        [StringLength(512)]
        public string ChangeContent { get; set; }
        /// <summary>
        /// 与业务表关联
        /// </summary>
        [StringLength(127)]
        public string ChangeId { get; set; }
        /// <summary>
        /// 变动原因（备注）
        /// </summary>
        [StringLength(512)]
        public string ChangeReason { get; set; }
        /// <summary>
        /// 决定人、发起人
        /// </summary>
        [StringLength(127)]
        public string UserId { get; set; }
        
    }
}
