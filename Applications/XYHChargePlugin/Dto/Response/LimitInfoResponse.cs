using System;
using System.Collections.Generic;
using System.Text;

namespace XYHChargePlugin.Dto
{
    public class LimitInfoResponse
    {
        public string UserId { get; set; }

        public int LimitType { get; set; }

        public decimal Amount { get; set; }

        public string Memo { get; set; }

        public string CreateUser { get; set; }

        public DateTime? CreateTime { get; set; }

        public string UpdateUser { get; set; }

        public DateTime? UpdateTime { get; set; }

        public bool IsDeleted { get; set; }

        public string DeleteUser { get; set; }

        public DateTime? DeleteTime { get; set; }

        
        public HumanInfoResponse UserInfo { get; set; }
        
        public decimal UsedAmount { get; set; }


        public string DepartmentName { get; set; }
    }
}
