using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    public class RelationHouse : TraceUpdateBase
    {
        /// <summary>
        /// 主键Guid
        /// </summary>
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RelationId { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// 房源Id
        /// </summary>
        public string HousingResourcesId { get; set; }
        /// <summary>
        /// 房源编号
        /// </summary>
        [MaxLength(127)]
        public string HousingResourcesNo { get; set; }
        /// <summary>
        /// 房源名称
        /// </summary>
        [MaxLength(255)]
        public string PropertyName { get; set; }
        /// <summary>
        /// 单元编号
        /// </summary>
        [MaxLength(255)]
        public string BuildUnitName { get; set; }
        /// <summary>
        /// 房间编号（后期可以加商铺编号）
        /// </summary>
        [MaxLength(127)]
        public string RoomNo { get; set; }
        /// <summary>
        /// 时间区间，例：2017-01-01至2017-12-31。未用
        /// </summary>
        [MaxLength(255)]
        public string HouseType { get; set; }
        /// <summary>
        /// 面积
        /// </summary>
        public decimal Acreage { get; set; }
        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
        /// <summary>
        /// 总价
        /// </summary>
        public decimal AllPrice { get; set; }
        /// <summary>
        /// 未用
        /// </summary>
        public int LookResult { get; set; }
        /// <summary>
        /// 未用
        /// </summary>
        public int NoLook { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(1024)]
        public string Remark { get; set; }
        /// <summary>
        /// 反馈时间
        /// </summary>
        public DateTime? FeedbackTime { get; set; }
        /// <summary>
        /// 获得信息途径
        /// </summary>
        [MaxLength(255)]
        public string SearchMethod { get; set; }
        /// <summary>
        /// 业主电话
        /// </summary>
        [MaxLength(255)]
        public string OwnerPhone { get; set; }
        /// <summary>
        /// 业主姓名
        /// </summary>
        [MaxLength(50)]
        public string OwnerName { get; set; }
        /// <summary>
        /// 房源添加时间
        /// </summary>
        public DateTime? HouseAddTime { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 类型Id(123),暂未用
        /// </summary>
        public string TypeId { get; set; }
        /// <summary>
        /// 楼盘驻场用户
        /// </summary>
        [MaxLength(127)]
        public string InSiteUserId { get; set; }
        /// <summary>
        /// 驻场部门
        /// </summary>
        [MaxLength(127)]
        public string InSiteDepartmentId { get; set; }

        /// <summary>
        /// 商铺编号
        /// </summary>
        [MaxLength(255)]
        public string ShopNumber { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        [MaxLength(255)]
        public string AreaFullName { get; set; }

        /// <summary>
        /// 图片链接
        /// </summary>
        [MaxLength]
        public string ImageUrl { get; set; }
        /// <summary>
        /// 图片数量
        /// </summary>
        public int? ImageCount { get; set; }


        [NotMapped]
        public string CustomerName { get; set; }

        [NotMapped]
        public string MainPhone { get; set; }

        [NotMapped]
        public string UserId { get; set; }
    }
}
