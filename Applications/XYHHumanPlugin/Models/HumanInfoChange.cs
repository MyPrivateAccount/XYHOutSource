using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 人事变动信息
    /// </summary>
    public class HumanInfoChange
    {
        /// <summary>e
        /// 主键Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 人事Id
        /// </summary>
        [MaxLength(127)]
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
        [MaxLength(512)]
        public string ChangeContent { get; set; }
        /// <summary>
        /// 与业务表关联
        /// </summary>
        [MaxLength(127)]
        public string ChangeId { get; set; }
        /// <summary>
        /// 变动原因（备注）
        /// </summary>
        [MaxLength(512)]
        public string ChangeReason { get; set; }
        /// <summary>
        /// 决定人、发起人
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }

        public DateTime CreateTime { get; set; }
        [MaxLength(127)]
        public string CreateUser { get; set; }
        [MaxLength(127)]
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }


    }

    /// <summary>
    /// 人事变动类型
    /// </summary>
    public enum HumanChangeType
    {
        /// <summary>
        /// 入职
        /// </summary>
        Entry = 1,
        /// <summary>
        /// 转正
        /// </summary>
        Regular = 2,
        /// <summary>
        /// 异动调薪
        /// </summary>
        Adjustment = 3,
        /// <summary>
        /// 兼职
        /// </summary>
        Parttime = 4,
        /// <summary>
        /// 离职
        /// </summary>
        Leave = 5
    }
}
