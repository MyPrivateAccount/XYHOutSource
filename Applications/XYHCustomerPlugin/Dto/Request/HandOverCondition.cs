using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    /// <summary>
    /// 客户转移条件
    /// </summary>
    public class HandOverCondition
    {
        /// <summary>
        /// 需转移人ID
        /// </summary>
        public string OldUserId { get; set; }

        /// <summary>
        /// 接受人
        /// </summary>
        public string NewUserId { get; set; }

        /// <summary>
        /// 接受人部门Id
        /// </summary>
        public string NewDepatmentId { get; set; }

        /// <summary>
        /// 接受人部门名称
        /// </summary>
        public string NewDepatmentName { get; set; }

        /// <summary>
        /// 是否移交跟进信息
        /// </summary>
        public bool IsHandOverFU { get; set; }
    }
}
