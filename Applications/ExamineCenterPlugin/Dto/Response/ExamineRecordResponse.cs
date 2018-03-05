using ExamineCenterPlugin.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExamineCenterPlugin.Dto
{
    public class ExamineRecordResponse
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public Int64 Sort { get; set; }
        /// <summary>
        /// 审核流程Id
        /// </summary>
        public string FlowId { get; set; }
        /// <summary>
        /// 审核用户
        /// </summary>
        public string ExamineUserId { get; set; }
        /// <summary>
        /// 审核内容
        /// </summary>
        public string ExamineContents { get; set; }
        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime RecordTime { get; set; }
        /// <summary>
        /// 记录状态
        /// </summary>
        public RecordStatus RecordStstus { get; set; }
        public RecordTypes RecordType { get; set; }
        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? ExamineTime { get; set; }

        public bool IsCurrent { get; set; }
        public string ExamineUserName { get; set; }



    }
}
