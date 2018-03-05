using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingBaseInfoResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string District { get; set; }
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
        public double? plotRatio { get; set; }
        public double? GreeningRate { get; set; }
        public int? BasementParkingSpace { get; set; }
        public int? ParkingSpace { get; set; }
        public int? BuildingNum { get; set; }
        public int? Shops { get; set; }
        public string PMC { get; set; }
        public decimal? PMF { get; set; }
        public string Developer { get; set; }
        public string Address { get; set; }

        public string AreaFullName { get; set; }
    }
}
