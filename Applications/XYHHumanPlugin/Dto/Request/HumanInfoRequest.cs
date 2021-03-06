﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class HumanInfoRequest
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Required]
        [StringLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [StringLength(127)]
        public string Name { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        [StringLength(127)]
        public string UserID { get; set; }

        /// <summary>
        /// 入职类型
        /// </summary>
        [StringLength(255)]
        public string PositionType { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [StringLength(127)]
        public string IDCard { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public Int16 Sex { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        [StringLength(255)]
        public string Company { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        [StringLength(50)]
        public string Phone { get; set; }
        /// <summary>
        /// 名族
        /// </summary>
        [StringLength(255)]
        public string Nationality { get; set; }

        /// <summary>
        /// 户籍类型
        /// </summary>
        [StringLength(255)]
        public string HouseholdType { get; set; }

        /// <summary>
        /// 最高学历
        /// </summary>
        [StringLength(255)]
        public string HighestEducation { get; set; }

        /// <summary>
        /// 健康状况
        /// </summary>
        [StringLength(255)]
        public string HealthCondition { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        public string NativePlace { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        [StringLength(255)]
        public string FamilyAddress { get; set; }
        /// <summary>
        /// 婚姻状况
        /// </summary>
        public bool? MaritalStatus { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>
        [StringLength(255)]
        public string Position { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        public string PolicitalStatus { get; set; }

        /// <summary>
        /// 户籍所在地
        /// </summary>
        public string DomicilePlace { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        public string EmergencyContact { get; set; }

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        public string EmergencyContactPhone { get; set; }

        /// <summary>
        /// 紧急联系人关系
        /// </summary>
        public string EmergencyContactType { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        [StringLength(50)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string BankAccount { get; set; }

        /// <summary>
        /// 所属部门Id
        /// </summary>
        [StringLength(127)]
        public string DepartmentId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        [StringLength(127)]
        public string Picture { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime EntryTime { get; set; }


        public HumanSalaryStructureRequest HumanSalaryStructureRequest { get; set; }

        public HumanSocialSecurityRequest HumanSocialSecurityRequest { get; set; }

        public HumanContractInfoRequest HumanContractInfoRequest { get; set; }

        public IEnumerable<HumanTitleInfoRequest> HumanTitleInfosRequest { get; set; }
        public IEnumerable<HumanWorkHistoryRequest> HumanWorkHistoriesRequest { get; set; }

        public IEnumerable<HumanEducationInfoRequest> HumanEducationInfosRequest { get; set; }
    }
}
