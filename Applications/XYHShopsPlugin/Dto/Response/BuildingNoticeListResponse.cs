using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingNoticeListResponse
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        public string BuildingId { get; set; }
        public string Title { get; set; }
        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
        public string UserId { get; set; }
        public string OrganizationId { get; set; }

        public DateTime CreateTime { get; set; }
        public string Icon { get; set; }
        public string UserName { get; set; }
        public string OrganizationName { get; set; }

        public string BuildingName { get; set; }

        public string AreaFullName { get; set; }
    }
}
