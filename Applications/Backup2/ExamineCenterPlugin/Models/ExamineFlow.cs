using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExamineCenterPlugin.Models
{
    /// <summary>
    /// 审核流程表
    /// </summary>
    public class ExamineFlow
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 审核流程名
        /// </summary>
        [MaxLength(255)]
        public string Action { get; set; }
        /// <summary>
        /// NWF任务GUID
        /// </summary>
        [MaxLength(127)]
        public string TaskGuid { get; set; }
        /// <summary>
        /// 审核任务名字
        /// </summary>
        [MaxLength(255)]
        public string TaskName { get; set; }
        /// <summary>
        /// 当前步骤Id
        /// </summary>
        [MaxLength(127)]
        public string CurrentStepId { get; set; }
        /// <summary>
        /// 审核提交时间
        /// </summary>
        public DateTime SubmitTime { get; set; }
        /// <summary>
        /// 审核提交人Id
        /// </summary>
        [MaxLength(127)]
        public string SubmitUserId { get; set; }

        /// <summary>
        /// 提交组织
        /// </summary>
        [MaxLength(127)]
        public string SubmitOrganizationId { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public ExamineStatus ExamineStatus { get; set; }
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
        [MaxLength(1024)]
        public string Desc { get; set; }
        /// <summary>
        /// 拓展字段
        /// </summary>
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

        [MaxLength(1024)]
        public string CallbackUrl { get; set; }
        public bool IsDeleted { get; set; }

        public string DeleteUserId { get; set; }
        public DateTime? DeleteTime { get; set; }

        [NotMapped]
        public IEnumerable<ExamineRecord> ExamineRecords { get; set; }
        [NotMapped]
        public string SubmitUserName { get; set; }
        [NotMapped]
        public string SubmitOrganizationName { get; set; }
        [NotMapped]
        public string DeleteUserName { get; set; }
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
