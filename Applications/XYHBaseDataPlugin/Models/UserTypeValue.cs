using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHBaseDataPlugin.Models
{
    public class UserTypeValue
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        public string Id { get; set; }

        /// <summary>
        /// 定义人
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }

        /// <summary>
        /// 定义类型
        /// </summary>
        [MaxLength(255)]
        public string Type { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [MaxLength(512)]
        public string Value { get; set; }
    }
}
