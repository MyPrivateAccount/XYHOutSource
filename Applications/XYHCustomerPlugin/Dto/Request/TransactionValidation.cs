using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    public class TransactionValidation
    {
        public string BuildingId { get; set; }

        public List<string> CustomerIds { get; set; }

        public DateTime ExpectedLookTime { get; set; }
    }
}
