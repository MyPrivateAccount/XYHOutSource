using ExamineCenterPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class ExamineNoticeResponse
    {
        public string Id { get; set; }
        public string FlowId { get; set; }

        public string RecordId { get; set; }
        public DateTime SubmitTime { get; set; }
        public string SubmitUserId { get; set; }
        public string SubmitUserName { get; set; }
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
        /// <summary>
        /// 供提交者回调时使用的Id
        /// </summary>
        public string SubmitDefineId { get; set; }
        public string Content { get; set; }
        public string TaskName { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public ExamineStatus ExamineStatus { get; set; }
        /// <summary>
        /// 审核描述
        /// </summary>
        public string Desc { get; set; }
        /// <summary>
        /// 提交组织
        /// </summary>
        public string SubmitOrganizationId { get; set; }
        public string SubmitOrganizationName { get; set; }

        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
        public string Ext3 { get; set; }
        public string Ext4 { get; set; }
        public string Ext5 { get; set; }
        public string Ext6 { get; set; }
        public string Ext7 { get; set; }
        public string Ext8 { get; set; }
        public NoticeStatus NoticeStatus { get; set; }
    }
}
