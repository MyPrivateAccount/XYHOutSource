using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Response
{
    public class CustomerHeavy
    {
        public string CustomerName { get; set; }

        public string MainPhone { get; set; }

        public List<FollowUpResponse> FollowUpResponses { get; set; }
    }
}
