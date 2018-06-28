using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 人事合同表
    /// </summary>
    public class HumanContractInfo
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        [MaxLength(50)]
        public string ContractNo { get; set; }

        /// <summary>
        /// 合同类型
        /// </summary>
        [MaxLength(255)]
        public string ContractType { get; set; }

        /// <summary>
        /// 合同签署日期
        /// </summary>
        public DateTime? ContractSignDate { get; set; }

        /// <summary>
        /// 合同有效日期
        /// </summary>
        public DateTime? ContractStartDate { get; set; }

        /// <summary>
        /// 合同到期日期
        /// </summary>
        public DateTime? ContractEndDate { get; set; }


    }
}
