using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    public class ChekPhoneHistory
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        public DateTime CheckTime { get; set; }

        [MaxLength(127)]
        public string CustomerId { get; set; }

        [MaxLength(127)]
        public string CheckUserId { get; set; }
    }
}
