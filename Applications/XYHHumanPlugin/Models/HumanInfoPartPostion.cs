using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 兼职表
    /// </summary>
    public class HumanInfoPartPostion
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
        /// 部门Id
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        [MaxLength(64)]
        public string Position { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(512)]
        public string Desc { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public bool IsCurrent { get; set; }


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
}
