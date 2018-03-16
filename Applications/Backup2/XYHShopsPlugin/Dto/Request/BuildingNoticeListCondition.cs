using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingNoticeListCondition
    {
        [StringLength(20)]
        public string KeyWord { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public List<string> UserIds { get; set; }
        public List<string> OrganizationIds { get; set; }
        public string DistrictCode { get; set; }
        public string AreaCode { get; set; }
        public string CityCode { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }


    }
}
