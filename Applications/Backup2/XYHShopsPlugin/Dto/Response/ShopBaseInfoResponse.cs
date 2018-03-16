using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class ShopBaseInfoResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string BuildingId { get; set; }
        public string BuildingNo { get; set; }
        public string FloorNo { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string SaleStatus { get; set; }
        public decimal? Price { get; set; }
        public decimal? GuidingPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public double? BuildingArea { get; set; }
        public double? HouseArea { get; set; }
        public int? Floors { get; set; }
        public string Toward { get; set; }
        public double? Width { get; set; }
        public double? Depth { get; set; }
        public double? Height { get; set; }
        public double? OutsideArea { get; set; }
        public bool? HasFree { get; set; }
        public double? FreeArea { get; set; }
        public bool? HasStreet { get; set; }
        public double? StreetDistance { get; set; }

        public bool? IsCorner { get; set; }
        public bool? IsFaceStreet { get; set; }
        public string ShopCategory { get; set; }




    }
}
