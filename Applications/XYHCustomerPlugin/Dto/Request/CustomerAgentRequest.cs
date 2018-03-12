using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerAgentRequest
    {
        [Required]
        [StringLength(10)]
        public string CustomerName { get; set; }
        [StringLength(15)]
        public string CustomerPhone { get; set; }
    }
}
