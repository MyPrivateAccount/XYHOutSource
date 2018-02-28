using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHBaseDataPlugin.Dto.Request
{
    public class UserTypeValueRequest
    {
        /// <summary>
        /// 定义类型
        /// </summary>
        [StringLength(255, ErrorMessage = "Type最大长度为255")]
        public string Type { get; set; }

        /// <summary>
        /// 值
        /// </summary>
        [StringLength(512, ErrorMessage = "Type最大长度为512")]
        public string Value { get; set; }
    }
}
