using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 社保信息
    /// </summary>
    public class HumanSocialSecurity
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 人事Id
        /// </summary>
        public string HumanId { get; set; }

        /// <summary>
        /// 参保时间
        /// </summary>
        public DateTime? InsuredTime { get; set; }

        /// <summary>
        /// 参保地址
        /// </summary>
        public string InsuredAddress { get; set; }

        /// <summary>
        /// 是否放弃购买
        /// </summary>
        public bool IsGiveUp { get; set; } = false;

        /// <summary>
        /// 是否签订承诺书
        /// </summary>
        public bool IsSignCommitment { get; set; } = false;

        /// <summary>
        /// 医保账号
        /// </summary>
        public string MedicalInsuranceAccount { get; set; }

        /// <summary>
        /// 社保账号
        /// </summary>
        public string SocialSecurityAccount { get; set; }

        /// <summary>
        /// 住房公积金账号
        /// </summary>
        public string HousingProvidentFundAccount { get; set; }


    }
}
