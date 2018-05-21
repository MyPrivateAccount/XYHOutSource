using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto.Response
{
    public class LimitInfoResponse
    {
        public string UserID { get; set; }
        public int? LimitType { get; set; }
        public int? CostLimit { get; set; }
        public string ContentLimit { get; set; }
    }
}
