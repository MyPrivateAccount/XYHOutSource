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
        public int Age { get; set; }
        public int Sex { get; set; }
        public string Position { get; set; }
        public string PositionName { get; set; }
        public string DepartmentId { get; set; }
        public int? Payment { get; set; }
        public int? Modify { get; set; }
        public string Picture { get; set; }
        public string RecentModify { get; set; }
        public int? StaffStatus { get; set; }
        public string Contract { get; set; }
        public DateTime? EntryTime { get; set; }
        public DateTime? BecomeTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public bool? IsSocialInsurance { get; set; }
        public string SocialInsuranceInfo { get; set; }
        public int? BaseSalary { get; set; }
        public int? Subsidy { get; set; }
        public int? ClothesBack { get; set; }
        public int? AdministrativeBack { get; set; }
        public int? PortBack { get; set; }
        public int? OtherBack { get; set; }

        public string OrganizationFullName { get; set; }

        public PositionInfoResponse PositionInfo { get; set; }
    }

    public class HumanInfoFormResponse
    {
        public string ID { get; set; }
        public string UserID { get; set; }
        public string Name { get; set; }
        public string IDCard { get; set; }
        public int Age { get; set; }
        public string SexName { get; set; }//转换
        public string PositionName { get; set; }//转换

        public string DepartmentName { get; set; }//转换
        public string StaffStatusName { get; set; }//转换
        public string Contract { get; set; }//转换
        public DateTime? EntryTime { get; set; }
        public DateTime? BecomeTime { get; set; }
        public DateTime? LeaveTime { get; set; }
        public string SocialInsuranceInfo { get; set; }//转换
        public int? BaseSalary { get; set; }
        public int? Subsidy { get; set; }
        public int? ClothesBack { get; set; }
        public int? AdministrativeBack { get; set; }
        public int? PortBack { get; set; }
        public int? OtherBack { get; set; }
    }
}
