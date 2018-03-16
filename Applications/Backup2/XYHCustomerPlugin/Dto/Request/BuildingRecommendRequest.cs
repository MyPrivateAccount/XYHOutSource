using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    public class BuildingRecommendRequest
    {
        public List<ShopItem> Shops { get; set; }

        /// <summary>
        /// 不关心的楼盘或商铺
        /// </summary>
        public List<NotCareItem> NotCareItems { get; set; }

        public string BuildingId { get; set; }

        public string City { get; set; }

        public string District { get; set; }

        public string Area { get; set; }
    }

    public class ShopItem
    {
        public string Id { get; set; }
        public string FloorNo { get; set; }
       
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
       
        public double? StreetDistance { get; set; }
        public bool? IsCorner { get; set; }
        public bool? IsFaceStreet { get; set; }
       
        public string ShopCategory { get; set; }

        public DateTime? DeliveryDate { get; set; }
    }


    public class NotCareItem
    {
        
        public string CustomerId { get; set; }

        
        public string ShopsId { get; set; }

       
        public string UserId { get; set; }

        public DateTime? CreateDate { get; set; }

    
        public int? Type { get; set; }
    }
}
