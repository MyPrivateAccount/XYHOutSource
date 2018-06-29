using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 黑名单
    /// </summary>
    public class HumanInfoBlack
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 工号（非必须）
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [MaxLength(127)]
        public string IDCard { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        [MaxLength(127)]
        public string Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(50)]
        public string Email { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public Int16 Sex { get; set; } = 1;
        /// <summary>
        /// 加入原因
        /// </summary>
        [MaxLength(500)]
        public string Reason { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
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
