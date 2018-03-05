using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHShopsPlugin.Models
{
    public class BuildingShopInfo
    {
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        [MaxLength(255)]
        public string SaleStatus { get; set; } //销售状态
        [MaxLength(255)]
        public string ShopCategory { get; set; } //商铺类别
        [MaxLength(255)]
        public string SaleMode { get; set; }//销售模式
        
        public long? Populations { get; set; } //居住人口
        public string PreferentialPolicies { get; set; } //优惠政策

        [MaxLength(1024)]
        public string TradeMixPlanning { get; set; } //业态规划
    }
}
