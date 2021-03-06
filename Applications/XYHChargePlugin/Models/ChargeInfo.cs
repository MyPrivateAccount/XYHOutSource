﻿using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHChargePlugin.Models
{
    public class ChargeInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }

        [MaxLength(127)]
        public string BranchId { get; set; }

        [MaxLength(64)]
        public string BranchPrefix { get; set; }

        [MaxLength(64)]
        [ConcurrencyCheck()]
        public string ChargeNo { get; set; }

        
        public int Seq { get; set; }

        public int Type { get; set; }

        [MaxLength(127)]
        public string ReimburseDepartment { get; set; }

        [MaxLength(127)]
        public string ReimburseUser { get; set; }

        [MaxLength(255)]
        public string Payee { get; set; }



        [MaxLength(127)]
        public string Department { get; set; }

        public decimal ChargeAmount { get; set; }

        public bool IsPayment { get; set; }

        public decimal PaymentAmount { get; set; }

        public DateTime? PaymentTime { get; set; }

        public bool IsBackup { get; set; }

        public bool Backuped { get; set; }


        [MaxLength(255)]
        public string Memo { get; set; }


        public int BillCount { get; set; }

        public decimal BillAmount { get; set; }

        public int Status { get; set; }

        public int BillStatus { get; set; }



        public DateTime? CreateTime { get; set; }

        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DeleteTime { get; set; }

        public String DeleteUser { get; set; }

        public string SubmitUser { get; set; }

        public DateTime? SubmitTime { get; set; }

        public string ConfirmMessage { get; set; }

        public string ConfirmBillMessage { get; set; }

        public string ChargeId { get; set; }

        public DateTime? ExpectedPaymentDate { get; set; }

        public decimal? ReimbursedAmount { get; set; }

        public bool? IsReimbursed { get; set; }

        public DateTime? LastReimbursedTime { get; set; }

        public int RecordingStatus { get; set; }

        [NotMapped]
        public Organizations BranchInfo { get; set; }

        [NotMapped]
        public Organizations Organizations { get; set; }

        [NotMapped]
        public OrganizationExpansion OrganizationExpansion { get; set; }

        [NotMapped]
        public SimpleUser CreateUserInfo { get; set; }

        [NotMapped]
        public List<CostInfo> FeeList { get; set; }

        [NotMapped]
        public List<ReceiptInfo> BillList { get; set; }

        [NotMapped]
        public HumanInfo ReimburseUserInfo { get; set; }

        [NotMapped]
        public List<ModifyInfo> History { get; set; }

        [NotMapped]
        public List<ChargeInfo> ChargeList { get; set; }
        //public DateTime? PostTime { get; set; }
        //[MaxLength(127)]
        // public string PostDepartment { get; set; }

        // [MaxLength(127)]
        //  public string CreateUser { get; set; }
        // [MaxLength(127)]
        //  public string CreateUserName { get; set; }
        // [MaxLength(127)]
        //   public string CurrentModify { get; set; }
        //   public int TotalCost { get; set; }
    }
}
