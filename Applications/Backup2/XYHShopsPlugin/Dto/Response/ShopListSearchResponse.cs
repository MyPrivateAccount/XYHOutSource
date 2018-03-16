using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class ShopListSearchResponse
    {
        public string Id { get; set; }
        public string BuildingId { get; set; }
        public string BuildingName { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AreaFullName { get; set; }
        public decimal? Price { get; set; }
        public double? BuildingArea { get; set; }
        public decimal? TotalPrice { get; set; }

        public string BuildingNo { get; set; }
        public string FloorNo { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string SaleStatus { get; set; }

        public double? Width { get; set; }
        public double? Depth { get; set; }
        public double? Height { get; set; }

        public string Icon { get; set; }
        public bool? UpperWater { get; set; }// 是否有上水
        public bool? DownWater { get; set; }// 是否有下水
        public bool? Gas { get; set; }// 是否有天然气
        public bool? Chimney { get; set; }// 是否有烟管道
        public bool? Blowoff { get; set; }//排污管道
        public bool? Split { get; set; }// 可分割
        public bool? Elevator { get; set; }//电梯
        public bool? Staircase { get; set; }//扶梯
        public bool? Outside { get; set; }//外摆区
        public bool? OpenFloor { get; set; }// 架空层
        public bool? ParkingSpace { get; set; }//停车位
        public bool? IsCorner { get; set; }//拐角铺
        public bool? IsFaceStreet { get; set; }//是否临街
        public bool? HasFree { get; set; }
        public bool? HasStreet { get; set; }

        public DateTime? LockTime { get; set; }//锁定时间

        /// <summary>
        /// 是否热卖
        /// </summary>
        public bool? IsHot { get; set; }
    }
}
