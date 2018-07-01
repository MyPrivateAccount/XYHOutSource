using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class HumanInfoAdjustmentResponse
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
        /// 部门Id
        /// </summary>
        public string DepartmentId { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        public string Position { get; set; }

        /// <summary>
        /// 调整生效时间
        /// </summary>
        public DateTime AdjustmentTime { get; set; }

        /// <summary>
        /// 是否参加社保
        /// </summary>
        public bool IsHave { get; set; }
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
        /// 应发工资
        /// </summary>
        public decimal GrossPay { get; set; } = 0;
        /// <summary>
        /// 试用期工资
        /// </summary>
        public decimal ProbationaryPay { get; set; } = 0;

        public bool IsCurrent { get; set; }

        public DateTime CreateTime { get; set; }
        public string CreateUser { get; set; }
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
        public bool IsDeleted { get; set; }
    }
}
