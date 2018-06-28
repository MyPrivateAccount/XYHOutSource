using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 工作经历
    /// </summary>
    public class HumanWorkHistory
    {
        /// <summary>
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
        /// 公司名称
        /// </summary>
        [MaxLength(255)]
        public string Company { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [MaxLength(255)]
        public string Position { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>

        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        [MaxLength(127)]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(127)]
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }
    }
}
