using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Dto.Response
{
    public class HumanInfoChangeResponse
    {
        /// <summary>e
        /// 主键Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 人事Id
        /// </summary>
        public string HumanId { get; set; }
        /// <summary>
        /// 变动类型
        /// </summary>
        public HumanChangeType ChangeType { get; set; }
        /// <summary>
        /// 变动时间
        /// </summary>
        public DateTime ChangeTime { get; set; }
        /// <summary>
        /// 变动内容
        /// </summary>
        public string ChangeContent { get; set; }
        /// <summary>
        /// 与业务表关联
        /// </summary>
        public string ChangeId { get; set; }
        /// <summary>
        /// 变动原因（备注）
        /// </summary>
        public string ChangeReason { get; set; }
        /// <summary>
        /// 决定人、发起人
        /// </summary>
        public string UserId { get; set; }

        public DateTime CreateTime { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
