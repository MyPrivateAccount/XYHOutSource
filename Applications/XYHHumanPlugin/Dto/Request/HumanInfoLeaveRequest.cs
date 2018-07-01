using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    /// <summary>
    /// 离职
    /// </summary>
    public class HumanInfoLeaveRequest
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [StringLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 人事Id
        /// </summary>
        [StringLength(127)]
        public string HumanId { get; set; }
        /// <summary>
        /// 离职时间
        /// </summary>
        public DateTime LeaveTime { get; set; }
        /// <summary>
        /// 交接人
        /// </summary>
        [StringLength(127)]
        public string NewHumanId { get; set; }

        /// <summary>
        /// 是否办理离职手续
        /// </summary>
        public bool IsProcedure { get; set; }
        
    }
}
