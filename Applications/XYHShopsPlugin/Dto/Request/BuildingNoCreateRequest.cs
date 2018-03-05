using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto.Request
{
    public class BuildingNoCreateRequest
    {
        public string Storied { get; set; }
        
        public DateTime? OpenDate { get; set; }
        
        public DateTime? DeliveryDate { get; set; }
    }
}
