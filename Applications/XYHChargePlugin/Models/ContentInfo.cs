using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHChargePlugin.Models
{
    //public class CostInfo
    //{
    //    [Key]
    //    [MaxLength(127)]
    //    public string ID { get; set; }
    //    [MaxLength(127)]
    //    public string ChargeID { get; set; }
    //    public int? Type { get; set; }
    //    public int? Cost { get; set; }
    //    [MaxLength(255)]
    //    public string Comments { get; set; }
    //}

    //public class ReceiptInfo
    //{
    //    [Key]
    //    [MaxLength(127)]
    //    public string ID { get; set; }
    //    [MaxLength(127)]
    //    public string ChargeID { get; set; }
    //    [MaxLength(127)]
    //    public string CostID { get; set; }
    //    [MaxLength(127)]
    //    public string ReceiptNumber { get; set; }
    //    public int Type { get; set; }
    //    public int? ReceiptMoney { get; set; }
    //    [MaxLength(127)]
    //    public string Comments { get; set; }
    //    [MaxLength(127)]
    //    public string CreateUser { get; set; }
    //    public DateTime? CreateTime { get; set; }
    //}

    public class FileScopeInfo
    {
        [Key]
        [MaxLength(127)]
        public string ReceiptID { get; set; }

        [MaxLength(127)]
        public string FileGuid { get; set; }
    }

    public class LimitInfo
    {
        [Key]
        [MaxLength(127)]
        public string UserID { get; set; }
        public int? LimitType { get; set; }
        public int? CostLimit { get; set; }
    }

    public class ModifyInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string ChargeID { get; set; }
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
