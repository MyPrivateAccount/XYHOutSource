using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class TaskCallback
    {
        public string TaskGuid { get; set; }
        public string StepID { get; set; }
        public string Message { get; set; }
        public FlowProtocol CallbackProtocol { get; set; }
        public TaskStatusEnum Status { get; set; }
    }

    public class FlowProtocol
    {
        public string Protocol { get; set; }
        public string ProtocolType { get; set; }
    }

    public enum TaskStatusEnum
    {
        Waiting = 0, //等待 尚未分配到执行机器
        Scheduling = 1, //分配中，指正在分配给执行器
        Queuing = 2, //排队等待执行
        Executing = 3, //执行中
        Pausing = 4, //暂停中
        Paused = 5, //暂停
        Resuming = 6, //恢复中
        Retrying = 7, //重试中
        WaitingCallback = 8, //等待回调
        Failed = 9, //失败
        PartialFailed = 10, //部分失败 指有步骤失败，但其他分支步骤还在执行
        Finished = 11, //完成
        Cancel = 12, //取消
        Deleting = 13, //正在删除
        Deleted = 14, //删除
        UnSet = 100  //表示未设置
    }

}
