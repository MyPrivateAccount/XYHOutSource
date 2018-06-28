using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    public class ContentInfo
    {

    }


    public class AnnexInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string FileGuid { get; set; }
        [MaxLength(255)]
        public string From { get; set; }
        [MaxLength(255)]
        public string Group { get; set; }
        [MaxLength(127)]
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        [MaxLength(127)]
        public string UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }
    }

    /// <summary>
    /// 黑名单
    /// </summary>
    public class BlackInfo
    {
        [Key]
        [MaxLength(127)]
        public string IDCard { get; set; }
        [MaxLength(127)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string Reason { get; set; }
    }

    /// <summary>
    /// 考勤信息
    /// </summary>
    public class AttendanceInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        /// <summary>
        /// 工号
        /// </summary>
        public string UserID { get; set; }
        /// <summary>
        /// 考勤月份
        /// </summary>
        public DateTime? Date { get; set; }
        [MaxLength(32)]
        public string Name { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(255)]
        public string Comments { get; set; }
        /// <summary>
        /// 正常出勤天数
        /// </summary>
        public int Normal { get; set; }
        /// <summary>
        /// json详细信息
        /// </summary>
        [MaxLength(400)]
        public string NormalDate { get; set; }
        /// <summary>
        /// 调休
        /// </summary>
        public int Relaxation { get; set; }
        [MaxLength(400)]
        public string RelaxationDate { get; set; }
        /// <summary>
        /// 事假
        /// </summary>
        public int Matter { get; set; }
        [MaxLength(400)]
        public string MatterDate { get; set; }
        /// <summary>
        /// 病假
        /// </summary>
        public int Illness { get; set; }
        [MaxLength(400)]
        public string IllnessDate { get; set; }
        /// <summary>
        /// 年假
        /// </summary>
        public int Annual { get; set; }
        [MaxLength(400)]
        public string AnnualDate { get; set; }
        /// <summary>
        /// 婚假
        /// </summary>
        public int Marry { get; set; }
        [MaxLength(400)]
        public string MarryDate { get; set; }
        /// <summary>
        /// 丧假
        /// </summary>
        public int Funeral { get; set; }
        [MaxLength(400)]
        public string FuneralDate { get; set; }
        /// <summary>
        /// 迟到
        /// </summary>
        public int Late { get; set; }
        [MaxLength(400)]
        public string LateDate { get; set; }
        /// <summary>
        /// 旷工
        /// </summary>
        public int Absent { get; set; }
        [MaxLength(400)]
        public string AbsentDate { get; set; }
    }

    /// <summary>
    /// 考勤规则设置
    /// </summary>
    public class AttendanceSettingInfo
    {
        [Key]
        public int Type { get; set; }
        /// <summary>
        /// 次数
        /// </summary>
        public int Times { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Money { get; set; }
    }


    public class PositionInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string PositionName { get; set; }
        [MaxLength(127)]
        public string PositionType { get; set; }

        [MaxLength(255)]
        public string ParentID { get; set; }
    }

    public class SalaryInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string Organize { get; set; }
        [MaxLength(127)]
        public string Position { get; set; }
        [MaxLength(127)]
        public string PositionName { get; set; }
        /// <summary>
        /// 基本工资
        /// </summary>
        public int? BaseSalary { get; set; }
        /// <summary>
        /// 岗位补贴
        /// </summary>
        public int? Subsidy { get; set; }
        public int? ClothesBack { get; set; }
        public int? AdministrativeBack { get; set; }
        public int? PortBack { get; set; }
        public int? OtherBack { get; set; }
    }


    public class MonthInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        public DateTime? SettleTime { get; set; }
        [MaxLength(127)]
        public string OperName { get; set; }
        [MaxLength(127)]
        public string AttendanceForm { get; set; }
        [MaxLength(127)]
        public string SalaryForm { get; set; }
    }

    public class SalaryFormInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string MonthID { get; set; }
        [MaxLength(127)]
        public string HumanID { get; set; }
        public int? BaseSalary { get; set; }
        public int? Subsidy { get; set; }
        public int? ClothesBack { get; set; }
        public int? AdministrativeBack { get; set; }
        public int? PortBack { get; set; }
        public int? OtherBack { get; set; }
    }

    public class AttendanceFormInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string MonthID { get; set; }
        [MaxLength(127)]
        public string HumanID { get; set; }
        public DateTime? ComeTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public int? Late { get; set; }
        public int? Leave { get; set; }
    }

    public class ModifyInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string IDCard { get; set; }
        public int? Type { get; set; }
        public int? ExamineStatus { get; set; }
        public DateTime? ExamineTime { get; set; }
        [MaxLength(32)]
        public string ModifyPepole { get; set; }
        public DateTime? ModifyStartTime { get; set; }
        [MaxLength(127)]
        public string ModifyCheck { get; set; }
        [MaxLength(4000)]
        public string Ext1 { get; set; }
        [MaxLength(15000)]
        public string Ext2 { get; set; }
        [MaxLength(600)]
        public string Ext3 { get; set; }
        [MaxLength(100)]
        public string Ext4 { get; set; }
    }

    public class SocialInsurance
    {
        [Key]
        [MaxLength(127)]
        public string IDCard { get; set; }
        public bool IsSocial { get; set; }
        public bool Giveup { get; set; }
        public bool GiveupSign { get; set; }
        public DateTime? EnTime { get; set; }
        public DateTime? SureTime { get; set; }
        [MaxLength(255)]
        public string EnPlace { get; set; }
        public int? Pension { get; set; }
        public int? Medical { get; set; }
        public int? WorkInjury { get; set; }
        public int? Unemployment { get; set; }
        public int? Fertility { get; set; }
    }

    public class LeaveInfo
    {
        [Key]
        [MaxLength(127)]
        public string IDCard { get; set; }
        public DateTime? LeaveTime { get; set; }
        public bool IsFormalities { get; set; }
        public bool IsReduceSocialEnsure { get; set; }
    }

    public class ChangeInfo
    {
        [Key]
        [MaxLength(127)]
        public string IDCard { get; set; }
        public DateTime? ChangeTime { get; set; }
        public int? ChangeType { get; set; }
        public int? ChangeReason { get; set; }
        [MaxLength(127)]
        public string OtherReason { get; set; }
        [MaxLength(127)]
        public string OrgDepartmentId { get; set; }
        [MaxLength(127)]
        public string OrgPosition { get; set; }
        [MaxLength(127)]
        public string NewPosition { get; set; }
        [MaxLength(127)]
        public string NewDepartmentId { get; set; }
        public int? BaseSalary { get; set; }
        public int? Subsidy { get; set; }
        public int? ClothesBack { get; set; }
        public int? AdministrativeBack { get; set; }
        public int? PortBack { get; set; }
        public int? OtherBack { get; set; }
    }
}
