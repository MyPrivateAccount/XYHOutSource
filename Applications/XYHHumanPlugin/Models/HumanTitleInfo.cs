using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 职称信息
    /// </summary>
    public class HumanTitleInfo
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
        /// 职称名称
        /// </summary>
        [MaxLength(50)]
        public string TitleName { get; set; }

        /// <summary>
        /// 获得时间
        /// </summary>
        public DateTime? GetTitleTime { get; set; }

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
