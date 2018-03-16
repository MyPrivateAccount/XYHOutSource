using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto.Request
{
    /// <summary>
    /// 排序
    /// </summary>
    public class SequencingCondition
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否顺序
        /// </summary>
        public bool Value { get; set; }
    }
}
