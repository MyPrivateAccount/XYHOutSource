using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHChargePlugin.Models
{
    public class HumanInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string UserID { get; set; }
        [MaxLength(127)]
        public string Name { get; set; }
        
        public string DepartmentId { get; set; }
       
    }
}
