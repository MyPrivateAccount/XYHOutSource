using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHChargePlugin.Models
{
    public class ChargeInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        
        [MaxLength(127)]
        public string Department { get; set; }
        [MaxLength(255)]
        public string Note { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? PostTime { get; set; }
        [MaxLength(127)]
        public string PostDepartment { get; set; }
        
        [MaxLength(127)]
        public string CreateUser { get; set; }
        [MaxLength(127)]
        public string CreateUserName { get; set; }
    }
}
