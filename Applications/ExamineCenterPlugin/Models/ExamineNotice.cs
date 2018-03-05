using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ExamineCenterPlugin.Models
{
    /// <summary>
    /// 审核告知表
    /// </summary>
    public class ExamineNotice
    {
        /// <summary>
        /// 主键Guid
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 审核流程表主键
        /// </summary>
        [MaxLength(127)]
        public string FlowId { get; set; }

        /// <summary>
        /// 审核记录表主键
        /// </summary>
        [MaxLength(127)]
        public string RecordId { get; set; }

        /// <summary>
        /// 告知用户Id
        /// </summary>
        [MaxLength(127)]
        public string NoticeUserId { get; set; }

        /// <summary>
        /// 通知时间
        /// </summary>
        public DateTime NoticeTime { get; set; }

        /// <summary>
        /// 通知状态（已通知未查看，已通知已查看）
        /// </summary>
        public NoticeStatus NoticeStatus { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime? DeleteTime { get; set; }

        public string DeleteUserId { get; set; }

        [NotMapped]
        public ExamineFlow ExamineFlow { get; set; }

        [NotMapped]
        public ExamineRecord ExamineRecord { get; set; }

        [NotMapped]
        public string NoticeUserName { get; set; }

        [NotMapped]
        public string DeleteUserName { get; set; }
    }

    public enum NoticeStatus
    {
        Noticed = 1,
        Read = 2
    }



}
