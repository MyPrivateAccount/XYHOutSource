using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class HumanInfoResponse
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string IDCard { get; set; }
        public string Position { get; set; }
        public int? Payment { get; set; }
        public int? Modify { get; set; }
        public string Picture { get; set; }
        public string RecentModify { get; set; }
        public string Contract { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? BecomeTime { get; set; }
        public bool? IsSocialInsurance { get; set; }
        public string SocialInsuranceInfo { get; set; }
    }
}
