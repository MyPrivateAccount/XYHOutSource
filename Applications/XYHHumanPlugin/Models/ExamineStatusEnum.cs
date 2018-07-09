using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 审核状态
    /// </summary>
    public enum ExamineStatusEnum
    {
        /// <summary>
        /// 未提交
        /// </summary>
        UnSubmit = 0,
        /// <summary>
        /// 审核中
        /// </summary>
        Auditing = 1,
        /// <summary>
        /// 审核通过
        /// </summary>
        Approved = 8,
        /// <summary>
        /// 驳回
        /// </summary>
        Reject = 16
    }
}
