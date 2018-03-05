using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter.Models
{
    public class Shops
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        [MaxLength(256)]
        public string Name { get; set; }
        public string BuildingId { get; set; }
        public string BuildingNo { get; set; }
        public int Floor { get; set; }
        public string FloorNo { get; set; }
        public string Number { get; set; }
        public string Status { get; set; }
        public string SaleStatus { get; set; }
        public string Price { get; set; }
        public string TotalPrice { get; set; }
        public string BuildingArea { get; set; }
        public string HouseArea { get; set; }
        public string Toward { get; set; }
        public string Width { get; set; }
        public string Depth { get; set; }
        public string Height { get; set; }
        public string OutsideArea { get; set; }
        public bool HasFree { get; set; }
        public string FreeArea { get; set; }
        public string HasStreet { get; set; }
        public string streetDistance { get; set; }
        public string IsCorner { get; set; }
        public string IsFaceStreet { get; set; }
        /// <summary>
        /// 商铺类型
        /// </summary>
        public string ShopType { get; set; }


    }
}
