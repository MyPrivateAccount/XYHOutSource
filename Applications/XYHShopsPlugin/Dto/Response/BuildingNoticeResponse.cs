using System;
using System.Collections.Generic;
using System.Text;

namespace XYHShopsPlugin.Dto
{
    public class BuildingNoticeResponse
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 楼盘Id
        /// </summary>
        public string BuildingId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        public string Ext1 { get; set; }

        public string Ext2 { get; set; }
        /// <summary>
        /// 发布用户
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 发布组织
        /// </summary>
        public string OrganizationId { get; set; }

        public bool IsDeleted { get; set; }


        public string DeleteUser { get; set; }

        public DateTime? DeleteTime { get; set; }

        public string Icon { get; set; }

        public string BuildingName { get; set; }

        public string AreaFullName { get; set; }

        public string UserName { get; set; }
        public string OrganizationName { get; set; }
        public string DeleteUserName { get; set; }
        public List<FileItemResponse> FileList { get; set; }
        public List<AttachmentResponse> AttachmentList { get; set; }
    }
}
