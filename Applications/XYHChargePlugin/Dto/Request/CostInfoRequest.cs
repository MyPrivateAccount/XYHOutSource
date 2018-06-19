using System;

namespace XYHChargePlugin.Dto
{
    public class CostInfoRequest
    {
       
        public string Id { get; set; }

       
        public string ChargeId { get; set; }

        public int Type { get; set; }

        public decimal Amount { get; set; }

        public string Memo { get; set; }
    }
}
