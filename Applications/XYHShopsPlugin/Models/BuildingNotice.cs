using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHShopsPlugin.Models
{
    public class BuildingNotice
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 楼盘Id
        /// </summary>
        [MaxLength(127)]
        public string BuildingId { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        [MaxLength(255)]
        public string Title { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        [MaxLength(1024)]
        public string Ext1 { get; set; }

        [MaxLength(1024)]
        public string Ext2 { get; set; }
        /// <summary>
        /// 发布用户
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }
        /// <summary>
        /// 发布组织
        /// </summary>
        [MaxLength(127)]
        public string OrganizationId { get; set; }

        public DateTime CreateTime { get; set; }
        public bool IsDeleted { get; set; }

        [MaxLength(127)]
        public string DeleteUser { get; set; }

        public DateTime? DeleteTime { get; set; }

        [MaxLength(1024)]
        public string Icon { get; set; }


        [NotMapped]
        public IEnumerable<FileInfo> RecordFileInfos { get; set; }

        [NotMapped]
        public string UserName { get; set; }
        [NotMapped]
        public string OrganizationName { get; set; }

        //新加字段
        [NotMapped]
        public string BuildingName { get; set; }

        [NotMapped]
        public AreaDefine CityDefine { get; set; }

        [NotMapped]
        public AreaDefine DistrictDefine { get; set; }

        [NotMapped]
        public AreaDefine AreaDefine { get; set; }

    }
}
