using System;
using System.Collections.Generic;
using System.Text;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Dto
{
    public class UpdateRecordResponse
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 动态类型
        /// </summary>
        public UpdateType UpdateType { get; set; }

        /// <summary>
        /// 关联内容Id
        /// </summary>
        public string ContentId { get; set; }

        /// <summary>
        /// 更改用户
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 更改时间
        /// </summary>
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// 更新内容
        /// </summary>
        public string UpdateContent { get; set; }
        /// <summary>
        /// 动态标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 动态内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 内容类型
        /// </summary>
        public string ContentType { get; set; }

        public string Ext1 { get; set; }
        public string Ext2 { get; set; }
        public string Ext3 { get; set; }
        public string Ext4 { get; set; }
        public string Ext5 { get; set; }
        public string Ext6 { get; set; }
        public string Ext7 { get; set; }
        public string Ext8 { get; set; }
        public string BuildingName { get; set; }

        public string AreaFullName { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public ExamineStatusEnum ExamineStatus { get; set; }
        /// <summary>
        /// 提交审核时间
        /// </summary>
        public DateTime SubmitTime { get; set; }
        public string Icon { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime? DeleteTime { get; set; }
        public string DeleteUserId { get; set; }
        public string UserName { get; set; }
        public string OrganizationName { get; set; }
        public List<FileItemResponse> FileList { get; set; }
        public List<AttachmentResponse> AttachmentList { get; set; }
    }
}
