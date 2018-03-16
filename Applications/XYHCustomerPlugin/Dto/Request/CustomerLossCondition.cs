using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerLossCondition
    {

        public string KeyWords { get; set; }

        public List<LossType> Types { get; set; }

        public DateTime? LossTimeStart { get; set; }
        public DateTime? LossTimeEnd { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }

    }
}
