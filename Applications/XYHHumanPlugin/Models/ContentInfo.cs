using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHHumanPlugin.Models
{
    public class ContentInfo
    {
    }

    public class ContractInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string HumanID { get; set; }
        [MaxLength(255)]
        public string ContentPath { get; set; }
        [MaxLength(255)]
        public string ContentInfo { get; set; }
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

    public class AttendanceInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        public DateTime? Time { get; set; }
        [MaxLength(127)]
        public string Name { get; set; }
        [MaxLength(127)]
        public string IDCard { get; set; }
        [MaxLength(255)]
        public string History { get; set; }
    }

    public class PositionInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string PositionName { get; set; }
        [MaxLength(127)]
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
        public string PositionID { get; set; }
        public int? BaseSalary { get; set; }
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
        public DateTime? OperTime { get; set; }
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
}
