﻿using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerPoolJoinRequest
    {
        public List<string> CustomerIds { get; set; }

        public string DepartmentId { get; set; }
    }
}
