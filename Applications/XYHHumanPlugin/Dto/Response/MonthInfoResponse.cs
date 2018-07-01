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
        public string OperID { get; set; }
        public string AttendanceForm { get; set; }
        public string SalaryForm { get; set; }
    }

    public class SearchMonthInfoResponse
    {
        public int pageIndex { get; set; }
        public int pageSize { get; set; }
        public int totalCount { get; set; }
        public DateTime? lastTime { get; set; }
        public List<MonthInfoResponse> extension { get; set; }
    }
}
