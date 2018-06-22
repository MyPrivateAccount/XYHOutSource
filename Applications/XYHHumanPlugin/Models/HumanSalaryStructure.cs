using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 薪资构成
    /// </summary>
    public class HumanSalaryStructure
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
        /// 基本工资
        /// </summary>
        public decimal BaseWages { get; set; } = 0;
        /// <summary>
        /// 岗位工资
        /// </summary>
        public decimal PostWages { get; set; } = 0;
        /// <summary>
        /// 交通补贴
        /// </summary>
        public decimal TrafficAllowance { get; set; } = 0;
        /// <summary>
        /// 通讯补贴
        /// </summary>
        public decimal CommunicationAllowance { get; set; } = 0;
        /// <summary>
        /// 其他补贴
        /// </summary>
        public decimal OtherAllowance { get; set; } = 0;
        /// <summary>
        /// 养老保险
        /// </summary>
        public decimal EndowmentInsurance { get; set; } = 0;
        /// <summary>
        /// 医疗保险
        /// </summary>
        public decimal MedicalInsurance { get; set; } = 0;
        /// <summary>
        /// 失业保险
        /// </summary>
        public decimal UnemploymentInsurance { get; set; } = 0;
        /// <summary>
        /// 工伤保险
        /// </summary>
        public decimal EmploymentInjuryInsurance { get; set; } = 0;
        /// <summary>
        /// 生育保险
        /// </summary>
        public decimal MaternityInsurance { get; set; } = 0;
        /// <summary>
        /// 住房公积金
        /// </summary>
        public decimal HousingProvidentFund { get; set; } = 0;


    }
}
