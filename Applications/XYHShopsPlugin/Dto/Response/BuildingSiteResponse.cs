using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHShopsPlugin.Dto.Response
{
    /// <summary>
    /// 获取楼盘驻场
    /// </summary>
    public class BuildingSiteResponse
    {
        public string Id { get; set; }
        public int? ExamineStatus { get; set; }
        public string BuildingName { get; set; }
        public string ResidentUser1 { get; set; }
        public string ResidentUser2 { get; set; }
        public string ResidentUser3 { get; set; }
        public string ResidentUser4 { get; set; }
        public string Icon { get; set; }
        public string OrganizationId { get; set; }
        public SimpleUser ResidentUser1Info { get; set; }
        public SimpleUser ResidentUser2Info { get; set; }
        public SimpleUser ResidentUser3Info { get; set; }
        public SimpleUser ResidentUser4Info { get; set; }
    }
}
