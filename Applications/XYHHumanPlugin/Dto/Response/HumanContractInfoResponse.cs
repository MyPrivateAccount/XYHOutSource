using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class HumanContractInfoResponse
    {

        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContractNo { get; set; }

        /// <summary>
        /// 合同类型
        /// </summary>
        public string ContractType { get; set; }
        /// <summary>
        /// 签订合同单位
        /// </summary>
        public string ContractCompany { get; set; }
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
