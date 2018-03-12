using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerChangePoolRequest
    {
        [StringLength(127)]
        public List<string> CustomerIds { get; set; }
        [StringLength(127)]
        public string NewDepartmentId { get; set; }

    }
}
