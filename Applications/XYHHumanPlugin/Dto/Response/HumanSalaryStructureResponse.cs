using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class HumanSalaryStructureResponse
    {

        /// <summary>
        /// 基本工资
        /// </summary>
        public decimal BaseWages { get; set; }
        /// <summary>
        /// 岗位工资
        /// </summary>
        public decimal PostWages { get; set; }
        /// <summary>
        /// 交通补贴
        /// </summary>
        public decimal TrafficAllowance { get; set; }
        /// <summary>
        /// 通讯补贴
        /// </summary>
        public decimal CommunicationAllowance { get; set; }
        /// <summary>
        /// 其他补贴
        /// </summary>
        public decimal OtherAllowance { get; set; }
    }
}
