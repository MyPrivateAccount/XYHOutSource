using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Response
{
    public class CustomerSearchPool
    {
        public string Id { get; set; }

        public string CustomerName { get; set; }

        public string MainPhone { get; set; }

        public CustomerDemandResponse CustomerDemandResponse { get; set; }

        public string Source { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? FollowupTime { get; set; }

        public string Mark { get; set; }

        public string UserName { get; set; }

        public string UserId { get; set; }

        public string UserPhone { get; set; }

        public string DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public CustomerPoolResponse CustomerPoolResponse { get; set; }
    }
}
