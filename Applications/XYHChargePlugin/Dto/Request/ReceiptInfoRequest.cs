

using System;

namespace XYHChargePlugin.Dto
{
    public class ReceiptInfoRequest
    {
        
        public string Id { get; set; }

       
        public string ChargeId { get; set; }

        public string CostId { get; set; }

       
        public string ReceiptNumber { get; set; }

        public decimal ReceiptMoney { get; set; }

        public string Memo { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateTime { get; set; }

        
    }
}
