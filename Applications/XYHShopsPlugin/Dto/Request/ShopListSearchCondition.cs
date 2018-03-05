using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHShopsPlugin.Dto.Request;

namespace XYHShopsPlugin.Dto
{
    public class ShopListSearchCondition
    {
        [StringLength(20)]
        public string KeyWord { get; set; }
        public decimal? LowPrice { get; set; }
        public decimal? HighPrice { get; set; }

        public double? LowArea { get; set; }
        public double? HighArea { get; set; }


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

        public double? BuildingArea { get; set; }//建筑面积

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        

        public List<string> BuildingIds { get; set; }

        /// <summary>
        /// 查询客户ID
        /// </summary>
        public string CustomerId { get; set; }

        [StringLength(255)]
        public string City { get; set; }
        /// <summary>
        /// 区域
        /// </summary>
        [StringLength(255)]
        public string District { get; set; }
        /// <summary>
        /// 片区
        /// </summary>
        [StringLength(255)]
        public string Area { get; set; }
        /// <summary>
        /// True为从低到高，False为从高到低，Null为不按价格排序
        /// </summary>
        public bool? PriceIsAscSort { get; set; }
        /// <summary>
        /// True为从低到高，False为从高到低，Null为不按面积排序
        /// </summary>
        public bool? AreaIsAscSort { get; set; }
        
        public List<string> SaleStatus { get; set; } //租售状态    

        public List<SequencingCondition> SequencingConditions { get; set; }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
