using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Dto
{
    public class UpdateRecordRequest
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 动态类型
        /// </summary>
        public UpdateType UpdateType { get; set; }

        /// <summary>
        /// 关联内容Id
        /// </summary>
        [StringLength(127)]
        public string ContentId { get; set; }

        [StringLength(255)]
        public string ContentName { get; set; }
        /// <summary>
        /// 更新内容
        /// </summary>
        public string UpdateContent { get; set; }
        /// <summary>
        /// 动态标题
        /// </summary>
        [StringLength(50)]
        public string Title { get; set; }
        /// <summary>
        /// 动态内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 内容类型(字典数据)
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

        [StringLength(1024)]
        public string Image { get; set; }


    }
}
