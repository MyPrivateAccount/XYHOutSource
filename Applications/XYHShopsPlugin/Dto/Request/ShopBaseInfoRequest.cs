using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class ShopBaseInfoRequest
    {
        [StringLength(127)]
        public string Id { get; set; }
        [StringLength(255)]
        public string Name { get; set; }
        [StringLength(127)]
        public string BuildingId { get; set; }
        [StringLength(255)]
        public string BuildingNo { get; set; }
        [StringLength(255)]
        public string FloorNo { get; set; }
        [StringLength(255)]
        public string Number { get; set; }
        [StringLength(255)]
        public string Status { get; set; }

        public string SaleStatus { get; set; }
        public decimal? GuidingPrice { get; set; }
        public decimal? TotalPrice { get; set; }
        public double? BuildingArea { get; set; }
        public double? HouseArea { get; set; }
        public int? Floors { get; set; }
        [StringLength(255)]
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
        [StringLength(255)]
        public string ShopCategory { get; set; }

    }
}
