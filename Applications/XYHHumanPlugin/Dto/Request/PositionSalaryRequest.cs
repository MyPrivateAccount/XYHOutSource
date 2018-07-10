using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class PositionSalaryRequest
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [StringLength(127)]
        public string Id { get; set; }
        ///// <summary>
        ///// 所属部门
        ///// </summary>
        //[StringLength(127)]
        //public string DepartmentId { get; set; }
        ///// <summary>
        ///// 所属职位
        ///// </summary>
        //[StringLength(127)]
        //public string PositionId { get; set; }

        /// <summary>
        /// 基本工资
        /// </summary>
        public decimal? BaseWages { get; set; } = 0;
        /// <summary>
        /// 岗位工资
        /// </summary>
        public decimal? PostWages { get; set; } = 0;
        /// <summary>
        /// 交通补贴
        /// </summary>
        public decimal? TrafficAllowance { get; set; } = 0;
        /// <summary>
        /// 通讯补贴
        /// </summary>
        public decimal? CommunicationAllowance { get; set; } = 0;
        /// <summary>
        /// 其他补贴
        /// </summary>
        public decimal? OtherAllowance { get; set; } = 0;
    }
}
