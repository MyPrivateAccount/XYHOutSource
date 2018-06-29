using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class HumanInfoBlackRequest
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [StringLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 工号（非必须）
        /// </summary>
        [StringLength(127)]
        public string UserId { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [StringLength(50)]
        public string Name { get; set; }
        /// <summary>
        /// 身份证号
        /// </summary>
        [StringLength(127)]
        public string IDCard { get; set; }
        /// <summary>
        /// 电话号码
        /// </summary>
        [StringLength(127)]
        public string Phone { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [StringLength(50)]
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
    }
}
