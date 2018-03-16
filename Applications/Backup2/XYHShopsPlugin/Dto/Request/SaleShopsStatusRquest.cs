using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto.Request
{
    public class SaleShopsStatusRquest
    {
        public List<string> ShopsIds { get; set; }

        [StringLength(127, ErrorMessage = "SaleStatus不能超过255个字符")]
        public string SaleStatus { get; set; }

        [StringLength(127, ErrorMessage = "Mark不能超过1024个字符")]
        public string Mark { get; set; }

        public DateTime? LockTime { get; set; }
    }
}
