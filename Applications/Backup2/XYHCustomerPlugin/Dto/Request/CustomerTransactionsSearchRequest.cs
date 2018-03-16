using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Dto.Request
{
    public class CustomerTransactionsSearchRequest
    {
        [StringLength(127, ErrorMessage = "KeyWord最大长度为127")]
        public string KeyWord { get; set; }
        
        public List<int> Status { get; set; }

        [StringLength(127, ErrorMessage = "BuildingId最大长度为127")]
        public string BuildingId { get; set; }

        public bool ValPhone { get; set; }

        /// <summary>
        /// 是否只获取今日
        /// </summary>
        public bool? IsToDay { get; set; }

        /// <summary>
        /// 是否只获取今日
        /// </summary>
        public int? ReportEffectiveTime { get; set; }

        public DateTime? ReportStartTime { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; }
    }
}
