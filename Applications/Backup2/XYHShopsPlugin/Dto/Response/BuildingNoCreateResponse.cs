using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto.Response
{
    public class BuildingNoCreateResponse
    {
        public string BuildingId { get; set; }

        public string Storied { get; set; }

        public DateTime? OpenDate { get; set; }

        public DateTime? DeliveryDate { get; set; }
    }
}
