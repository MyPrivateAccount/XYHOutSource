using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;
using XYHHumanPlugin.Models;

namespace XYHHumanPlugin.Dto.Response
{
    public class HumanInfoResponse
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IDCard { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public Int16 Sex { get; set; }

        public DateTime Birthday { get; set; }

        /// <summary>
        /// 公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 名族
        /// </summary>
        public string Nationality { get; set; }

        /// <summary>
        /// 户籍类型
        /// </summary>
        public string HouseholdType { get; set; }

        /// <summary>
        /// 最高学历
        /// </summary>
        public string HighestEducation { get; set; }

        /// <summary>
        /// 健康状况
        /// </summary>
        public string HealthCondition { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        public string NativePlace { get; set; }

        /// <summary>
        /// 家庭住址
        /// </summary>
        public string FamilyAddress { get; set; }

        /// <summary>
        /// 婚姻状况
        /// </summary>
        public bool? MaritalStatus { get; set; }
        /// <summary>
        /// 职位名称
        /// </summary>
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
        public string DepartmentId { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateUser { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Desc { get; set; }

        public int? Modify { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Picture { get; set; }

        public string OrganizationFullName { get; set; }

        public PositionInfoResponse PositionInfo { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public ExamineStatusEnum ExamineStatus { get; set; } = ExamineStatusEnum.UnSubmit;

        /// <summary>
        /// 状态
        /// </summary>
        public StaffStatus StaffStatus { get; set; }

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime EntryTime { get; set; }
        /// <summary>
        /// 转正日期？
        /// </summary>
        public DateTime? BecomeTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }

        public bool IsDeleted { get; set; }
        public Organizations Organizations { get; set; }

        public OrganizationExpansion OrganizationExpansion { get; set; }

        public HumanContractInfoResponse HumanContractInfoResponse { get; set; }

        public HumanSalaryStructureResponse HumanSalaryStructureResponse { get; set; }

        public HumanSocialSecurityResponse HumanSocialSecurityResponse { get; set; }

        public IEnumerable<HumanEducationInfoResponse> HumanEducationInfosResponse { get; set; }

        public IEnumerable<HumanTitleInfoResponse> HumanTitleInfosResponse { get; set; }

        public IEnumerable<HumanWorkHistoryResponse> HumanWorkHistoriesResponse { get; set; }

        public IEnumerable<FileInfo> FileInfos { get; set; }




    }
}
