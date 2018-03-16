using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    public class CustomersTranRequest
    {
        public List<string> transactionsids { get; set; }

        public bool valphone { get; set; }
    }
}
