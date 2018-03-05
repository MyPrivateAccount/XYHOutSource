using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class ExamineSubmitRequest
    {
        /// <summary>
        /// 审核流程名
        /// </summary>
        [MaxLength(255)]
        public string Action { get; set; }
        /// <summary>
        /// 任务名字
        /// </summary>
        [MaxLength(1024)]
        public string TaskName { get; set; }
        /// <summary>
        /// 审核内容类型，字典数据
        /// </summary>
        [MaxLength(255)]
        public string ContentType { get; set; }
        /// <summary>
        /// 审核内容Id
        /// </summary>
        [MaxLength(127)]
        public string ContentId { get; set; }
        /// <summary>
        /// 审核内容名字
        /// </summary>
        [MaxLength(255)]
        public string ContentName { get; set; }

        public string Content { get; set; }
        /// <summary>
        /// 供提交者回调时使用的Id
        /// </summary>
        [MaxLength(127)]
        public string SubmitDefineId { get; set; }
        /// <summary>
        /// 审核描述
        /// </summary>
        [MaxLength(5000)]
        public string Desc { get; set; }
        /// <summary>
        /// 回调地址
        /// </summary>
        [MaxLength(1024)]
        public string CallbackUrl { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }

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
}
