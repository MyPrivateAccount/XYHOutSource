using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHHumanPlugin.Models
{
    public class HumanInfo
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 工号
        /// </summary>
        [MaxLength(127)]
        public string UserID { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(127)]
        public string Name { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [MaxLength(50)]
        public string Phone { get; set; }
        /// <summary>
        /// 身份证号码
        /// </summary>
        [MaxLength(50)]
        public string IDCard { get; set; }
        /// <summary>
        /// 性别
        /// </summary>
        public Int16 Sex { get; set; } = 2;

        /// <summary>
        /// 公司
        /// </summary>
        [MaxLength(255)]
        public string Company { get; set; }

        /// <summary>
        /// 名族
        /// </summary>
        [MaxLength(255)]
        public string Nationality { get; set; }

        /// <summary>
        /// 户籍类型
        /// </summary>
        [MaxLength(255)]
        public string HouseholdType { get; set; }

        /// <summary>
        /// 最高学历
        /// </summary>
        [MaxLength(255)]
        public string HighestEducation { get; set; }

        /// <summary>
        /// 健康状况
        /// </summary>
        [MaxLength(255)]
        public string HealthCondition { get; set; }

        /// <summary>
        /// 籍贯
        /// </summary>
        [MaxLength(255)]
        public string NativePlace { get; set; }
        /// <summary>
        /// 婚姻状况
        /// </summary>
        public bool? MaritalStatus { get; set; }
        /// <summary>
        /// 家庭住址
        /// </summary>
        [MaxLength(255)]
        public string FamilyAddress { get; set; }

        /// <summary>
        /// 职位名称
        /// </summary>
        [MaxLength(255)]
        public string Position { get; set; }

        /// <summary>
        /// 政治面貌
        /// </summary>
        [MaxLength(255)]
        public string PolicitalStatus { get; set; }

        /// <summary>
        /// 户籍所在地
        /// </summary>
        [MaxLength(255)]
        public string DomicilePlace { get; set; }

        /// <summary>
        /// 紧急联系人
        /// </summary>
        [MaxLength(50)]
        public string EmergencyContact { get; set; }

        /// <summary>
        /// 紧急联系电话
        /// </summary>
        [MaxLength(127)]
        public string EmergencyContactPhone { get; set; }

        /// <summary>
        /// 紧急联系人关系
        /// </summary>
        [MaxLength(50)]
        public string EmergencyContactType { get; set; }

        /// <summary>
        /// 邮件地址
        /// </summary>
        [MaxLength(50)]
        public string EmailAddress { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        [MaxLength(50)]
        public string BankName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        [MaxLength(50)]
        public string BankAccount { get; set; }

        /// <summary>
        /// 所属部门Id
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(512)]
        public string Desc { get; set; }


        /// <summary>
        /// 头像
        /// </summary>
        [MaxLength(255)]
        public string Picture { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int? Modify { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [MaxLength(127)]
        public string RecentModify { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [MaxLength(255)]
        public StaffStatus StaffStatus { get; set; } = 0;

        /// <summary>
        /// 入职日期
        /// </summary>
        public DateTime? EntryTime { get; set; }

        /// <summary>
        /// 转正日期？
        /// </summary>
        public DateTime? BecomeTime { get; set; }


        /// <summary>
        /// 离职时间？
        /// </summary>
        public DateTime? LeaveTime { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public int? ClothesBack { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? AdministrativeBack { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? PortBack { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int? OtherBack { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public ExamineStatusEnum ExamineStatus { get; set; } = ExamineStatusEnum.UnSubmit;

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
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
        public Organizations Organizations { get; set; }

        [NotMapped]
        public OrganizationExpansion OrganizationExpansion { get; set; }

        [NotMapped]
        public HumanContractInfo HumanContractInfo { get; set; }

        [NotMapped]
        public HumanSalaryStructure HumanSalaryStructure { get; set; }

        [NotMapped]
        public HumanSocialSecurity HumanSocialSecurity { get; set; }

        [NotMapped]
        public IEnumerable<HumanEducationInfo> HumanEducationInfos { get; set; }

        [NotMapped]
        public IEnumerable<HumanTitleInfo> HumanTitleInfos { get; set; }

        [NotMapped]
        public IEnumerable<HumanWorkHistory> HumanWorkHistories { get; set; }

        [NotMapped]
        public IEnumerable<FileInfo> FileInfos { get; set; }

        [NotMapped]
        public PositionInfo PositionInfo { get; set; }
    }

    /// <summary>
    /// 员工状态
    /// </summary>
    public enum StaffStatus
    {
        /// <summary>
        /// 未入职
        /// </summary>
        NonEntry = 1,

        /// <summary>
        /// 试用
        /// </summary>
        Entry = 2,

        /// <summary>
        /// 正式工
        /// </summary>
        Regular = 3,

        /// <summary>
        /// 离职
        /// </summary>
        Leave = 4,

        /// <summary>
        /// 黑名单
        /// </summary>
        Black = 10
    }

}
