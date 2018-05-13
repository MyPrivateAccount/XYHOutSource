using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto.Response
{
    public class CostInfoResponse
    {
        public string ID { get; set; }
        public string ChargeID { get; set; }
        public int? Type { get; set; }
        public int? Cost { get; set; }
        public string Comments { get; set; }
    }
}
