using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    /// <summary>
    /// 兼职
    /// </summary>
    public class HumanInfoPartPositionRequest
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
        /// 部门Id
        /// </summary>
        [StringLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 职位
        /// </summary>
        [StringLength(64)]
        public string Position { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(512)]
        public string Desc { get; set; }






    }
}
