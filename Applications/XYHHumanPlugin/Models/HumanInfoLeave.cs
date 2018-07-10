using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 离职表
    /// </summary>
    public class HumanInfoLeave
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 人事Id
        /// </summary>
        [MaxLength(127)]
        public string HumanId { get; set; }
        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime LeaveTime { get; set; }
        /// <summary>
        /// 交接人
        /// </summary>
        [MaxLength(127)]
        public string NewHumanId { get; set; }

        /// <summary>
        /// 是否办理离职手续
        /// </summary>
        public bool IsProcedure { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public ExamineStatusEnum ExamineStatus { get; set; } = ExamineStatusEnum.UnSubmit;
        public bool IsCurrent { get; set; }

        public DateTime CreateTime { get; set; }
        [MaxLength(127)]
        public string CreateUser { get; set; }
        [MaxLength(127)]
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
        [MaxLength(127)]
        public string DeleteUser { get; set; }
        public DateTime? DeleteTime { get; set; }

        [NotMapped]
        public string OrganizationId { get; set; }
    }
}
