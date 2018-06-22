using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class CostInfoResponse
    {
        public string Id { get; set; }


        public string ChargeId { get; set; }

        public int Type { get; set; }

        public decimal Amount { get; set; }

        public string Memo { get; set; }
    }

    //public class CostInfoResponseEx
    //{
    //    public string ID { get; set; }
    //    public string ChargeID { get; set; }
    //    public int? Type { get; set; }
    //    public int? Cost { get; set; }
    //    public string Comments { get; set; }
    //    public List<ReceiptInfoResponse> ReceiptList { get; set; }
    //}
}
