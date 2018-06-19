using ApplicationCore.Dto;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class ChargeInfoResponse
    {
        public string ID { get; set; }


        public string BranchId { get; set; }

        public string BranchPrefix { get; set; }

        public string ChargeNo { get; set; }


        public int Seq { get; set; }

        public int Type { get; set; }


        public string ReimburseDepartment { get; set; }


        public string ReimburseUser { get; set; }


        public string Payee { get; set; }

        public string Department { get; set; }

        public decimal ChargeAmount { get; set; }

        public bool IsPayment { get; set; }

        public decimal PaymentAmount { get; set; }

        public bool IsBackup { get; set; }

        public string Memo { get; set; }


        public int BillCount { get; set; }

        public decimal BillAmount { get; set; }

        public int Status { get; set; }

        public int BillStatus { get; set; }

        public DateTime? CreateTime { get; set; }

        public string CreateUser { get; set; }

        public string UpdateUser { get; set; }

        public DateTime? UpdateTime { get; set; }


        public List<CostInfoResponse> FeeList { get; set; }

        public List<ReceiptInfoResponse> BillList { get; set; }

        public UserInfo CreateUserInfo { get; set; }

        public string BranchName { get; set; }

        public string ReimburseDepartmentName { get; set; }

        public UserInfo ReimburseUserInfo { get; set; }
    }
}
