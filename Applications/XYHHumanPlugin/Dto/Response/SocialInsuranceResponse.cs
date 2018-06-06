using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class SocialInsuranceResponse
    {
        public string IDCard { get; set; }
        public bool IsSocial { get; set; }
        public bool Giveup { get; set; }
        public bool GiveupSign { get; set; }
        public DateTime? EnTime { get; set; }
        public string EnPlace { get; set; }
        public int? Pension { get; set; }
        public int? Medical { get; set; }
        public int? WorkInjury { get; set; }
        public int? Unemployment { get; set; }
        public int? Fertility { get; set; }
    }
}
