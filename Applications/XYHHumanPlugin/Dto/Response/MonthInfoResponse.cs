using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class MonthInfoResponse
    {
        public string ID { get; set; }
        public DateTime? SettleTime { get; set; }
        public string OperName { get; set; }
        public DateTime? OperTime { get; set; }
    }
}
