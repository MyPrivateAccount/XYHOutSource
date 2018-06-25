using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public enum BillStatusEnum
    {
        UnSubmit = 0,
        Submit = 4, //已提交
        Confirm = 8, //已确认
        Reject = 16 //驳回
    }
}
