using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingSearchCondition
    {
        [StringLength(20)]
        public string KeyWord { get; set; }
        public decimal? LowPrice { get; set; }
        public decimal? HighPrice { get; set; }

        public bool? HasBus { get; set; }  //公交
        public bool? HasRail { get; set; } //轨道交通
        public bool? HasOtherTraffic { get; set; } //其他交通
        public bool? HasKindergarten { get; set; } //幼儿园
        public bool? HasPrimarySchool { get; set; } //小学
        public bool? HasMiddleSchool { get; set; } //中学
        public bool? HasUniversity { get; set; } //大学
        public bool? HasMarket { get; set; } //商场
        public bool? HasSupermarket { get; set; } //超市
        public bool? HasBank { get; set; } //银行


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
        
        public List<string> SaleStatus { get; set; } //销售状态

        [StringLength(127)]
        public string ResidentUser { get; set; }
        public List<int?> ExamineStatus { get; set; }
    }
}
