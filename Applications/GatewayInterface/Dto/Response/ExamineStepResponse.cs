using System;
using System.Collections.Generic;
using System.Text;

namespace GatewayInterface.Dto.Response
{
    public class ExamineStepResponse
    {
        /// <summary>
        /// 审核流程Id
        /// </summary>
        public string FlowId { get; set; }
        /// <summary>
        /// 审核用户Id
        /// </summary>
        public string ExamineUserId { get; set; }
        /// <summary>
        /// 审核用户工号
        /// </summary>
        public string ExamineUserName { get; set; }
        /// <summary>
        /// 审核用户真实姓名
        /// </summary>
        public string ExamineUserTrueName { get; set; }

        /// <summary>
        /// 审核内容Id
        /// </summary>
        public string ContentId { get; set; }
        /// <summary>
        /// 审核内容类型
        /// </summary>
        public string ContentType { get; set; }
        /// <summary>
        /// 审核内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 供提交者回调时使用的Id
        /// </summary>
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
