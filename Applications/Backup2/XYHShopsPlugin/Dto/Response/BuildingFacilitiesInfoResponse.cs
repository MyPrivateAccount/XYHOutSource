using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingFacilitiesInfoResponse
    {
        public string Id { get; set; }
        public bool? HasBus { get; set; }  //公交
        public string BusDesc { get; set; } //公交描述
        public bool? HasRail { get; set; } //轨道交通
        public string RailDesc { get; set; }
        public bool? HasOtherTraffic { get; set; } //其他交通
        public string OtherTrafficDesc { get; set; }
        public bool? HasKindergarten { get; set; } //幼儿园
        public string KindergartenDesc { get; set; }
        public bool? HasPrimarySchool { get; set; } //小学
        public string PrimarySchoolDesc { get; set; }
        public bool? HasMiddleSchool { get; set; } //中学
        public string MiddleSchoolDesc { get; set; }
        public bool? HasUniversity { get; set; } //大学
        public string UniversityDesc { get; set; }
        public bool? HasMarket { get; set; } //商场
        public string MarketDesc { get; set; }
        public bool? HasSupermarket { get; set; } //超市
        public string SupermarketDesc { get; set; }
        public bool? HasHospital { get; set; } //医院
        public string HospitalDesc { get; set; }
        public bool? HasBank { get; set; } //银行
        public string BankDesc { get; set; }
        public bool? HasOther { get; set; } //其他
        public string OtherDesc { get; set; }
    }
}
