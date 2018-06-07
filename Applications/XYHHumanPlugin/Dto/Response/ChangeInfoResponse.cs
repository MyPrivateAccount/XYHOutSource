using System;
using System.Collections.Generic;
using System.Text;

namespace XYHHumanPlugin.Dto.Response
{
    public class ChangeInfoResponse
    {
        public string ID {get;set;}//这个不是数据库的，专门用来放human表的id
        public string IDCard { get; set; }
        public DateTime? ChangeTime { get; set; }
        public int? ChangeType { get; set; }
        public int? ChangeReason { get; set; }
        public string OtherReason { get; set; }
        public string OrgDepartmentId { get; set; }
        public string OrgPosition { get; set; }
        public string NewPosition { get; set; }
        public string NewDepartmentId { get; set; }
        public int? BaseSalary { get; set; }
        public int? Subsidy { get; set; }
        public int? ClothesBack { get; set; }
        public int? AdministrativeBack { get; set; }
        public int? PortBack { get; set; }
        public int? OtherBack { get; set; }
    }
}
