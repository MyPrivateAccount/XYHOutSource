using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class CostSearchRequest
    {
        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string ReimburseDepartment { get; set; }

        public bool? IsPayment { get; set; }

        public bool? IsBackup { get; set; }

        public string Keyword { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}
