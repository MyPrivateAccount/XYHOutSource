using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHChargePlugin.Models
{
    public class ReceiptInfo
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        [MaxLength(127)]
        public string ChargeId { get; set; }

        public string CostId { get; set; }

        [MaxLength(64)]
        public string ReceiptNumber { get; set; }

        public decimal ReceiptMoney { get; set; }

        public string Memo { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateTime { get; set; }

        [NotMapped]
        public SimpleUser CreateUserInfo { get; set; }
    }
}
