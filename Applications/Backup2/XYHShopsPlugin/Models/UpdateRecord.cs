using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHShopsPlugin.Models
{
    /// <summary>
    /// 动态
    /// </summary>
    public class UpdateRecord
    {
        /// <summary>
        /// 主键Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 动态类型
        /// </summary>
        public UpdateType UpdateType { get; set; }

        /// <summary>
        /// 关联内容Id
        /// </summary>
        [MaxLength(127)]
        public string ContentId { get; set; }

        /// <summary>
        /// 更改用户
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }

        /// <summary>
        /// 更改组织
        /// </summary>
        [MaxLength(127)]
        public string OrganizationId { get; set; }
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
        [MaxLength(50)]
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

        /// <summary>
        /// 审核状态
        /// </summary>
        public ExamineStatusEnum ExamineStatus { get; set; }
        /// <summary>
        /// 提交审核时间
        /// </summary>
        public DateTime SubmitTime { get; set; }
        [MaxLength(1024)]
        public string Icon { get; set; }

        public bool IsDeleted { get; set; }
        public bool IsCurrent { get; set; }
        public DateTime? DeleteTime { get; set; }
        [MaxLength(127)]
        public string DeleteUserId { get; set; }

        [NotMapped]
        public IEnumerable<FileInfo> RecordFileInfos { get; set; }


        //新加字段
        [NotMapped]
        public string BuildingName { get; set; }

        [NotMapped]
        public AreaDefine CityDefine { get; set; }

        [NotMapped]
        public AreaDefine DistrictDefine { get; set; }

        [NotMapped]
        public AreaDefine AreaDefine { get; set; }

        [NotMapped]
        public string UserName { get; set; }

    }

    public enum UpdateType
    {
        Building = 1,
        Shops = 2
    }

    public enum ContentType
    {
        /// <summary>
        /// 热卖户型
        /// </summary>
        ShopsHot = 1,
        /// <summary>
        /// 加推
        /// </summary>
        ShopsAdd = 2,
        /// <summary>
        /// 报备规则
        /// </summary>
        ReportRule = 3,
        /// <summary>
        /// 佣金方案
        /// </summary>
        CommissionType = 4,
        /// <summary>
        /// 楼栋批次
        /// </summary>
        BuildingNo = 5,
        /// <summary>
        /// 优惠政策
        /// </summary>
        DiscountPolicy = 6,
        /// <summary>
        /// 图片
        /// </summary>
        Image = 7,
        /// <summary>
        /// 附件
        /// </summary>
        Attachment = 8,
        /// <summary>
        /// 价格
        /// </summary>
        Price = 9
    }



}
