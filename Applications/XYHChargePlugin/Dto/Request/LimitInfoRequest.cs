using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class LimitInfoRequest
    {
        public string UserId { get; set; }

        public int LimitType { get; set; }

        public decimal Amount { get; set; }

        public string Memo { get; set; }

        public bool IsDeleted { get; set; }


    }
}
