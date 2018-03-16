using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Models
{
    public class BuildingTradeMixPlanning
    {
        
        [MaxLength(127)]
        public string Id { get; set; }

       
        [MaxLength(255)]
        public string TradeMixPlanning { get; set; } //业态规划
      
    }
}
