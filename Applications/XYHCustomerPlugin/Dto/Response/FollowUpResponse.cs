using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Response
{
    public class FollowUpResponse
    {
        public CustomerFollowUpType TypeId { get; set; }

        public string TrueName { get; set; }

        public DateTime FollowUpTime { get; set; }

        public string FollowUpContents { get; set; }

        public Importance Importance { get; set; }

        public DemandLevel DemandLevel { get; set; }//需求等级
        
        public string FollowMode { get; set; }

        public string UserTrueName { get; set; }

        public string DepartmentName { get; set; }
    }
}
