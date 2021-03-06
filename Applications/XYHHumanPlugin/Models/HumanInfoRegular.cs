﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHHumanPlugin.Models
{
    /// <summary>
    /// 转正表
    /// </summary>
    public class HumanInfoRegular
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 人事Id
        /// </summary>
        [MaxLength(127)]
        public string HumanId { get; set; }
        /// <summary>
        /// 转正时间
        /// </summary>
        public DateTime RegularTime { get; set; }

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
        [MaxLength(255)]
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
        [MaxLength(127)]
        public string MedicalInsuranceAccount { get; set; }

        /// <summary>
        /// 社保账号
        /// </summary>
        [MaxLength(127)]
        public string SocialSecurityAccount { get; set; }

        /// <summary>
        /// 住房公积金账号
        /// </summary>
        [MaxLength(127)]
        public string HousingProvidentFundAccount { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public ExamineStatusEnum ExamineStatus { get; set; } = ExamineStatusEnum.UnSubmit;
        /// <summary>
        /// 是否有效
        /// </summary>
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
