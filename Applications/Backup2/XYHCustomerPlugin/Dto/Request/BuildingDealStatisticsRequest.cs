using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    public class BuildingDealStatisticsRequest
    {
        public string BuildingId { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }
    }
}
