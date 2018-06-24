using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class LimitTipResponse
    {
        public decimal LimitAmount { get; set; }

        public decimal UnSubmitAmount { get; set; }

        public decimal TotalAmount { get; set; }
    }
}
