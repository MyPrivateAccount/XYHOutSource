using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingShopInfoRequest
    {
        [StringLength(127)]
        public string Id { get; set; }

        public string SaleStatus { get; set; } //销售状态
        [StringLength(255)]
        public string ShopCategory { get; set; } //商铺类别
        [StringLength(255)]
        public string SaleMode { get; set; }//销售模式
       
        public List<string> TradeMixPlanning { get; set; } //业态规划
        public long? Populations { get; set; } //居住人口
        public string PreferentialPolicies { get; set; } //优惠政策
    }
}
