using System;
using System.Collections.Generic;

namespace XYHChargePlugin.Dto
{
    public class ChargeInfoRequest
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


        public List<CostInfoRequest> FeeList { get; set; }

        public List<ReceiptInfoRequest> BillList { get; set; }

    }
}
