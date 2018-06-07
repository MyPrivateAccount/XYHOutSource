using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class SocialInsuranceResponse
    {
        public string ID {get;set;}//这个不是数据库的，专门用来放human表的id
        public string IDCard { get; set; }
        public bool IsSocial { get; set; }
        public bool Giveup { get; set; }
        public bool GiveupSign { get; set; }
        public DateTime? EnTime { get; set; }
        public DateTime? SureTime { get; set; }
        public string EnPlace { get; set; }
        public int? Pension { get; set; }
        public int? Medical { get; set; }
        public int? WorkInjury { get; set; }
        public int? Unemployment { get; set; }
        public int? Fertility { get; set; }
    }
}
