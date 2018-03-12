using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto
{
    public enum ExamineStatusEnum
    {
        UnSubmit = 0, //未提交
        Auditing = 1, //审核中
        Approved = 8, //审核通过
        Reject = 16 //驳回
    }
}
