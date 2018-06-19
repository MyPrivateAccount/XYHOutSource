using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHChargePlugin.Models
{
    public class CostInfo
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        [MaxLength(127)]
        public string ChargeId { get; set; }

        public int Type { get; set; }

        public decimal Amount { get; set; }

        public string Memo { get; set; }
    }
}
