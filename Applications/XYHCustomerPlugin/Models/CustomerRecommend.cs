using System;
using System.Collections.Generic;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客源推荐（暂未用）
    /// </summary>
    public class CustomerRecommend
    {
        public string Id { get; set; }
        public string TypeId { get; set; }
        public string CustomerId { get; set; }
        public string HousingResourcesId { get; set; }
        public string CompanyId { get; set; }
        public string EmployeeId { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
