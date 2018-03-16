using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Dto
{
    public class ExamineResponse
    {
        /// <summary>
        /// 审核流程Id
        /// </summary>
        [MaxLength(127)]
        public string FlowId { get; set; }
        /// <summary>
        /// 审核内容Id
        /// </summary>
        [MaxLength(127)]
        public string ContentId { get; set; }

        [MaxLength(255)]
        public string ContentType { get; set; }

        public string Content { get; set; }
        /// <summary>
        /// 供提交者回调时使用的Id
        /// </summary>
        [MaxLength(127)]
        public string SubmitDefineId { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public ExamineStatus ExamineStatus { get; set; }

        [MaxLength(1024)]
        public string Ext1 { get; set; }
        [MaxLength(1024)]
        public string Ext2 { get; set; }
        [MaxLength(1024)]
        public string Ext3 { get; set; }
        [MaxLength(1024)]
        public string Ext4 { get; set; }
        [MaxLength(1024)]
        public string Ext5 { get; set; }
        [MaxLength(1024)]
        public string Ext6 { get; set; }
        [MaxLength(1024)]
        public string Ext7 { get; set; }
        [MaxLength(1024)]
        public string Ext8 { get; set; }
    }

    public enum ExamineStatus
    {
        /// <summary>
        /// 审核中
        /// </summary>
        Examining = 1,
        /// <summary>
        /// /审核完成
        /// </summary>
        Examined = 2,
        /// <summary>
        /// 驳回
        /// </summary>
        Reject = 3
    }
}

