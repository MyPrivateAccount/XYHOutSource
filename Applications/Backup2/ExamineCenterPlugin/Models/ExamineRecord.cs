using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExamineCenterPlugin.Models
{
    /// <summary>
    /// 审核记录表
    /// </summary>
    public class ExamineRecord
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 排序自动生成
        /// </summary>
        public Int64 Sort { get; set; }
        /// <summary>
        /// 审核流程Id
        /// </summary>
        [MaxLength(127)]
        public string FlowId { get; set; }
        /// <summary>
        /// 审核用户
        /// </summary>
        [MaxLength(127)]
        public string ExamineUserId { get; set; }
        /// <summary>
        /// 审核内容
        /// </summary>
        [MaxLength(5000)]
        public string ExamineContents { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime RecordTime { get; set; }
        /// <summary>
        /// 记录状态
        /// </summary>
        public RecordStatus RecordStstus { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ExamineTime { get; set; }
        public bool IsCurrent { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 删除用户
        /// </summary>
        public string DeleteUserId { get; set; }
        /// <summary>
        /// 删除时间
        /// </summary>
        public DateTime? DeleteTime { get; set; }

        public RecordTypes RecordType { get; set; }
        [NotMapped]
        public ExamineFlow ExamineFlow { get; set; }

        [NotMapped]
        public string ExamineUserName { get; set; }

        [NotMapped]
        public string DeleteUserName { get; set; }

    }

    public enum RecordTypes
    {
        /// <summary>
        /// 提交
        /// </summary>
        Submit = 1,
        /// <summary>
        /// 审核
        /// </summary>
        Examine = 2,
        /// <summary>
        /// 告知
        /// </summary>
        Notice = 3,
        /// <summary>
        /// 完成
        /// </summary>
        End = 4
    }

    public enum RecordStatus
    {
        /// <summary>
        /// 提交
        /// </summary>
        Submit = 1,
        /// <summary>
        /// 待审核
        /// </summary>
        Waiting = 2,
        /// <summary>
        /// 审核通过
        /// </summary>
        Examined = 3,
        /// <summary>
        /// 驳回
        /// </summary>
        Reject = 4,
        /// <summary>
        /// 完成
        /// </summary>
        Complete = 5,
    }

}
