using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class LimitSearchRequest
    {
        public string DepartmentId { get; set; }

        public string Keyword { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }
    }
}
