using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHChargePlugin.Models
{
    public class LimitInfo
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

        [NotMapped]
        public HumanInfo UserInfo { get; set; }
        [NotMapped]
        public decimal UsedAmount { get; set; }

        [NotMapped]
        public Organizations Organizations { get; set; }

        [NotMapped]
        public OrganizationExpansion OrganizationExpansion { get; set; }

    }
}
