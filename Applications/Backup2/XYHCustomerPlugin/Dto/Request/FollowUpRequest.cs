using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Request
{
    public class FollowUpRequest
    {
        public string CustomerId { get; set; }

        public Importance? Importance { get; set; }

        public DemandLevel? DemandLevel { get; set; }

        public string FollowUpContents { get; set; }

        public string FollowMode { get; set; }

        //public NeedHouseType HouseTypeId { get; set; }
    }
}
