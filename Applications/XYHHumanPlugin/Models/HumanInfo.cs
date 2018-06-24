﻿using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHHumanPlugin.Models
{
    public class HumanInfo
    {
        [Key]
        [MaxLength(127)]
        public string ID { get; set; }
        [MaxLength(127)]
        public string UserID { get; set; }
        [MaxLength(127)]
        public string Name { get; set; }
        [MaxLength(127)]
        public string IDCard { get; set; }
        public int? Age { get; set; }
        public int? Sex { get; set; }
        [MaxLength(127)]
        public string Position { get; set; }
        [MaxLength(127)]
        public string DepartmentId { get; set; }
        [MaxLength(127)]
        public string CreateUser { get; set; }
        public int? Modify { get; set; }
        [MaxLength(127)]
        public string Picture { get; set; }
        [MaxLength(127)]
        public string RecentModify { get; set; }
        public int? StaffStatus { get; set; }
        [MaxLength(127)]
        public string Contract { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? BecomeTime { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public bool? IsSocialInsurance { get; set; }
        [MaxLength(127)]
        public string SocialInsuranceInfo { get; set; }
        public int? BaseSalary { get; set; }
        public int? Subsidy { get; set; }
        public int? ClothesBack { get; set; }
        public int? AdministrativeBack { get; set; }
        public int? PortBack { get; set; }
        public int? OtherBack { get; set; }

        [NotMapped]
        public Organizations Organizations { get; set; }

        [NotMapped]
        public OrganizationExpansion OrganizationExpansion { get; set; }

        [NotMapped]
        public PositionInfo PositionInfo { get; set; }

    }
}
