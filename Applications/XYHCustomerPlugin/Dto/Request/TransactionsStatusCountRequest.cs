using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    public class TransactionsStatusCountRequest
    {
        public string Buildingid { get; set; }

        public bool? IsToday { get; set; }

        public int? ReportEffectiveTime { get; set; }
    }
}
