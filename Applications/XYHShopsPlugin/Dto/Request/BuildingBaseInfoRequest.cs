using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingBaseInfoRequest
    {
        [StringLength(127)]
        public string Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(255)]
        public string City { get; set; }
        [StringLength(255)]
        public string District { get; set; }
        [StringLength(255)]
        public string Area { get; set; }
        public int? HouseHolds { get; set; }
        public DateTime? OpenDate { get; set; }

        public DateTime? DeliveryDate { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public int? PropertyTerm { get; set; }
        public DateTime? LandExpireDate { get; set; }
        public double? FloorSurface { get; set; }
        public double? BuiltupArea { get; set; }
        public double? PlotRatio { get; set; }
        public double? GreeningRate { get; set; }
        public int? BasementParkingSpace { get; set; }
        public int? ParkingSpace { get; set; }
        public int? BuildingNum { get; set; }
        public int? Shops { get; set; }
        [StringLength(255)]
        public string PMC { get; set; }
        public decimal? PMF { get; set; }
        [StringLength(255)]
        public string Developer { get; set; }
        [StringLength(255)]
        public string Address { get; set; }

        public string ReportedRules { get; set; }

        public string CommissionPlan { get; set; }
    }
}
