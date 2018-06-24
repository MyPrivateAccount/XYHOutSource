using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        [NotMapped]
        public DictionaryDefine TypeInfo { get; set; }

        [NotMapped]
        public ChargeInfo ChargeInfo { get; set; }

        [NotMapped]
        public string TypeName { get; set; }
    }
}
