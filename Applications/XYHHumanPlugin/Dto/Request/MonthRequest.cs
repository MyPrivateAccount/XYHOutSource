using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Request
{
    public class MonthRequest
    {
        public string otherInfo { get; set; }
        public int OrderRule { get; set; }
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
    }
}
