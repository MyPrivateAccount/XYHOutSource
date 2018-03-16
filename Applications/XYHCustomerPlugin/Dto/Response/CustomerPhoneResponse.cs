using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Response
{
    public class CustomerPhoneResponse
    {
        public string Phone { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? DeleteTime { get; set; }

        public bool IsMain { get; set; }
    }
}
