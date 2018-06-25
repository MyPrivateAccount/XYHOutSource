using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHChargePlugin.Models
{
    public class PositionInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string PositionName { get; set; }
        [MaxLength(127)]
        public string PositionType { get; set; }

        [MaxLength(255)]
        public string ParentID { get; set; }
    }
}
