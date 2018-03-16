using System;
using System.Collections.Generic;
using System.Text;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Dto
{
    public class BuildingSearchResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string AreaFullName { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string Icon { get; set; }


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

        public DateTime? BeltLook { get; set; }

    }
}
