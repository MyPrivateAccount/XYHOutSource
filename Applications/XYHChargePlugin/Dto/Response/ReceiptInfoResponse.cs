using System;
using System.Collections.Generic;
using System.Text;
using XYHChargePlugin.Models;

namespace XYHChargePlugin.Dto.Response
{
    public class ReceiptInfoResponse
    {
        public string ID { get; set; }
        public string CostID { get; set; }
        public string ChargeID { get; set; }
        public string ReceiptNumber { get; set; }
        public int Type { get; set; }
        public int? ReceiptMoney { get; set; }
        public string Comments { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        public List<SimpleList> FileList { get; set; }
    }

    public class ReceiptInfoRequest
    {
        public string ID { get; set; }
        public string CostID { get; set; }
        public string ChargeID { get; set; }
        public string ReceiptNumber { get; set; }
        public int Type { get; set; }
        public int? ReceiptMoney { get; set; }
        public string Comments { get; set; }
        public string CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
    }
}
