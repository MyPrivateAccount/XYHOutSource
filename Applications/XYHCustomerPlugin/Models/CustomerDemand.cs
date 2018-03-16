using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客户需求信息
    /// </summary>
    public class CustomerDemand : TraceUpdateBase
    {
        /// <summary>
        /// 主键ID
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }
        /// <summary>
        /// 客户ID
        /// </summary>
        [MaxLength(127)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 需求类型,内部字典数据（1住宅求租2住宅求购3商业求租4商业求购）跟客户表中HouseTypeId同含义
        /// </summary>
        public NeedHouseType TypeId { get; set; }
        /// <summary>
        /// 需求类型,字典数据
        /// </summary>
        public string RequirementType { get; set; }
        /// <summary>
        /// 重要程度，内部字典数据（1重要 2一般3无价值）
        /// </summary>
        public Importance? Importance { get; set; }
        /// <summary>
        /// 房间数区间开始
        /// </summary>
        public int? RoomStart { get; set; }
        /// <summary>
        /// 房间数区间结束
        /// </summary>
        public int? RoomEnd { get; set; }
        /// <summary>
        /// 客厅数区间开始
        /// </summary>
        public int? LivingRoomStart { get; set; }
        /// <summary>
        /// 客厅数区间结束
        /// </summary>
        public int? LivingRoomEnd { get; set; }
        /// <summary>
        /// 面积区间开始
        /// </summary>
        public decimal? AcreageStart { get; set; }
        /// <summary>
        /// 面积区间结束
        /// </summary>
        public decimal? AcreageEnd { get; set; }
        /// <summary>
        /// 楼层区间开始
        /// </summary>
        public int? FloorNumStart { get; set; }
        /// <summary>
        /// 楼层区间结束
        /// </summary>
        public int? FloorNumEnd { get; set; }
        /// <summary>
        /// 装修情况，字典数据存GUID默认有（毛坯，简装，中装，精装，豪装）
        /// </summary>
        [MaxLength(127)]
        public string Renovation { get; set; }
        /// <summary>
        /// 朝向,字典数据存GUID，默认有（东南西北和组合的8个方向）
        /// </summary>
        [MaxLength(127)]
        public string Orientation { get; set; }
        /// <summary>
        /// 区域Id，区域表Id
        /// </summary>
        [MaxLength(127)]
        public string AreaId { get; set; }
        /// <summary>
        /// 商圈，商圈表Id，暂不使用
        /// </summary>
        [MaxLength(127)]
        public string DistrictId { get; set; }
        /// <summary>
        /// 价格区间开始
        /// </summary>
        public decimal? PriceStart { get; set; }
        /// <summary>
        /// 价格区间结束
        /// </summary>
        public decimal? PriceEnd { get; set; }
        /// <summary>
        /// 购房方式  1商贷  3全款
        /// </summary>
        [MaxLength(127)]
        public string PurchaseWay { get; set; }
        /// <summary>
        /// 首付
        /// </summary>
        public decimal? DownPayment { get; set; }
        /// <summary>
        /// 其他需求（未用）
        /// </summary>
        [MaxLength(1000)]
        public string OtherNeeds { get; set; }
        /// <summary>
        /// 购买动机,字典数据存GUID，多选逗号分隔，默认有（投资，开公司，做生意，开餐馆）
        /// </summary>
        [MaxLength(1000)]
        public string PurchaseMotivation { get; set; }
        /// <summary>
        /// 所属部门ID
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 所属用户Id
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }
        /// <summary>
        /// 合租的意图（未用）
        /// </summary>
        public int? FlatShareIntention { get; set; }
        /// <summary>
        /// 租期（未用）
        /// </summary>
        public int? LeaseTerm { get; set; }
        /// <summary>
        /// 支付租金（未用）
        /// </summary>
        public int? PayRent { get; set; }
        /// <summary>
        /// 紧急程度，内部字典数据， (0未选择 1一月以内 2一到三月 3三月以上)
        /// </summary>
        public EmergencyDegree? EmergencyDegree { get; set; }
        /// <summary>
        /// 家用电器(未用)
        /// </summary>
        [MaxLength(1000)]
        public string HomeAppliances { get; set; }
        /// <summary>
        /// 家具(未用)
        /// </summary>
        [MaxLength(1000)]
        public string Furniture { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(500)]
        public string Remark { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// 需求等级（A,B,C,D）
        /// </summary>
        public DemandLevel? DemandLevel { get; set; }
        /// <summary>
        /// 小地址Id
        /// </summary>
        [MaxLength(127)]
        public string SmallAreaId { get; set; }
        /// <summary>
        /// 省
        /// </summary>
        [MaxLength(127)]
        public string ProvinceId { get; set; }
        /// <summary>
        /// 市
        /// </summary>
        [MaxLength(127)]
        public string CityId { get; set; }
        /// <summary>
        /// 大门数量区间开始（未用）
        /// </summary>
        public decimal DoorWithStart { get; set; }
        /// <summary>
        /// 大门数量区间结束（未用）
        /// </summary>
        public decimal DoorWithEnd { get; set; }
        /// <summary>
        /// 单价（未用）
        /// </summary>
        public int UnitPrice { get; set; }
        /// <summary>
        /// 通途，字典数据，存GUID，默认有（办公，做买卖，餐饮，仓库，其他）
        /// </summary>
        [MaxLength(127)]
        public string UsesType { get; set; }
        /// <summary>
        /// 物业类型，字典数据，存GUID，默认有（写字楼、商铺）
        /// </summary>
        public string PropertyType { get; set; }
        /// <summary>
        /// 位置类型，字典数据，存GUID，多选逗号分隔，默认有 （商业街临街 市场 社区 建筑底层 商城 交通设施）
        /// </summary>
        [MaxLength(1000)]
        public string PositionType { get; set; }
        /// <summary>
        /// 位置名称
        /// </summary>
        [MaxLength(255)]
        public string AreaFullName { get; set; }
    }

    public enum Importance
    {
        Important = 1,
        General = 2,
        NoValue = 3,
    }

    public enum DemandLevel
    {
        Urgency = 1,
        General = 2,
        Suspend = 3,
    }

    public enum EmergencyDegree
    {
        NoChoiceIsBetween = 0,
        JanuaryWithin = 1,
        OneToMarch = 3,
        InTheAbove = 4
    }

}
