using System;
using System.Collections.Generic;
using System.Text;

namespace GatewayInterface.Dto
{
    public class ExamineSubmitRequest
    {
        /// <summary>
        /// 审核流程名
        /// </summary>
        public string Action { get; set; }
        /// <summary>
        /// 任务名字
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 审核内容类型，字典数据
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 审核内容Id
        /// </summary>
        public string ContentId { get; set; }
        /// <summary>
        /// 审核内容名字
        /// </summary>
        public string ContentName { get; set; }
        public string Content { get; set; }
        /// <summary>
        /// 审核描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        public string CallbackUrl { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        public string SubmitDefineId { get; set; }

        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
        public string Ext3 { get; set; }
        public string Ext4 { get; set; }
        public string Ext5 { get; set; }
        public string Ext6 { get; set; }
        public string Ext7 { get; set; }
        public string Ext8 { get; set; }

    }
}
