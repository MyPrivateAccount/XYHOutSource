using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto.Request
{
    public class LimitHumanRequest
    {
        public string ID { get; set; }
        public int LimitType { get; set; }
        public int CostLimit { get; set; }
    }
}
