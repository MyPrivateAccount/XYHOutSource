﻿using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace XYHContractPlugin.Models
{
   
    public class ComplementInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string ContractID { get; set; }
        public int? ContentID { get; set; }
        [MaxLength(255)]
        public string ContentInfo { get; set; }
    }

    public class AnnexInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string ContractID { get; set; }
        public int? Type { get; set; }
        [MaxLength(255)]
        public string Path { get; set; }
    }

    public class CheckInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string OriginID { get; set; }
        [MaxLength(127)]
        public string CheckID { get; set; }
        [MaxLength(32)]
        public string From { get; set; }
        [MaxLength(32)]
        public string To { get; set; }
        [MaxLength(32)]
        public string Current { get; set; }
    }

    public class EstateInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(255)]
        public string EstateName { get; set; }
        [MaxLength(255)]
        public string Developer { get; set; }
        [MaxLength(255)]
        public string Address { get; set; }
    }

    public class ModifyInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string ContractID { get; set; }
        public int? Type { get; set; }
        /// <summary>
        /// 0未 1审核中 2完成 3拒绝
        /// </summary>
        public int? ExamineStatus { get; set; }
        [MaxLength(32)]
        public string ModifyPepole { get; set; }
        public DateTime? ModifyStartTime { get; set; }
        [MaxLength(127)]
        public string ModifyCheck { get; set; }
    }
}
