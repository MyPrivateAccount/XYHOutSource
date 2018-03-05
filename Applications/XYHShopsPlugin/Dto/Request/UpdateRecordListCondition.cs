using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Dto
{
    public class UpdateRecordListCondition
    {
        [StringLength(20)]
        public string KeyWord { get; set; }
        public List<UpdateType> UpdateTypes { get; set; }

        public List<string> ContentTypes { get; set; }
        public bool? IsCurrent { get; set; }
        public List<string> ContentIds { get; set; }
        public List<Models.ExamineStatusEnum> ExamineStatus { get; set; }
        public string DistrictCode { get; set; }
        public string AreaCode { get; set; }
        public string CityCode { get; set; }
        public string UserId { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
