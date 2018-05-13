using System;
using System.Collections.Generic;
using System.Text;

using XYHChargePlugin.Dto.Response;

namespace XYHChargePlugin.Dto.Request
{
    public class ContentRequest
    {
        public ChargeInfoResponse ChargeInfo { get; set; }
        public List<CostInfoResponse> CostInfos { get; set; }
        public List<ReceiptInfoResponse> ReceiptInfos { get; set; }
    }
}
