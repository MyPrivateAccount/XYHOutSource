using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    public class MigrationPoolHistory
    {
        [MaxLength(127)]
        public string CustomerId { get; set; }

        [MaxLength(127)]
        public string OriginalDepartment { get; set; }

        [MaxLength(127)]
        public string TargetDepartment { get; set; }

        public DateTime MigrationTime { get; set; }
    }
}
