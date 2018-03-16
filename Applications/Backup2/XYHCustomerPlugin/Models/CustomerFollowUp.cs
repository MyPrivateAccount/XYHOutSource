using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客户跟进信息
    /// </summary>
    public class CustomerFollowUp : TraceUpdateBase
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 重要程度
        /// </summary>
        public Importance? Importance { get; set; }

        /// <summary>
        /// 重要等级
        /// </summary>
        public DemandLevel? DemandLevel { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        [MaxLength(127)]
        public string CustomerId { get; set; }
        /// <summary>
        /// 跟进类型，内部字典数据（1需求跟进2客户来电3看电话4客源修改5客户报备6客户带看7客户成交8激活客户）
        /// </summary>
        public CustomerFollowUpType TypeId { get; set; }
        /// <summary>
        /// 跟进人
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }
        /// <summary>
        /// 跟进部门
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 跟进时间
        /// </summary>
        public DateTime? FollowUpTime { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string TrueName { get; set; }
        /// <summary>
        /// 跟进内容
        /// </summary>
        public string FollowUpContents { get; set; }
        /// <summary>
        /// 需求房源类型，内部字典数据（1住宅求租2住宅求购 3商业求租4商业求购）同客户表中HouseTypeId同含义
        /// </summary>
        public NeedHouseType HouseTypeId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        [MaxLength(127)]
        public string CustomerNo { get; set; }
        /// <summary>
        /// 是否手动输入的跟进（还有系统自动生成的跟进信息）
        /// </summary>
        public bool IsRealFollow { get; set; }
        /// <summary>
        /// 跟进源，未确定，暂不用（012）
        /// </summary>
        public string FollowSource { get; set; }
        /// <summary>
        /// 跟进方式 字典数据
        /// </summary>
        public string FollowMode { get; set; }
        /// <summary>
        /// 是否一手房
        /// </summary>
        public bool? IsNewHourse { get; set; }
        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        [NotMapped]
        public string UserTrueName { get; set; }

        [NotMapped]
        public string DepartmentName { get; set; }
    }
    public enum CustomerFollowUpType
    {
        /// <summary>
        /// 写跟进
        /// </summary>
        WriteFollowUp = 0,

        /// <summary>
        /// 需求跟进
        /// </summary>
        NeedFollowUp = 1,

        /// <summary>
        /// 客户来电
        /// </summary>
        CustomerCalls = 2,

        /// <summary>
        /// 看电话
        /// </summary>
        SeePhone = 3,

        /// <summary>
        /// 客源修改
        /// </summary>
        CustomersModify = 4,

        /// <summary>
        /// 客户报备
        /// </summary>
        CustomerReported = 5,

        /// <summary>
        /// 客户带看
        /// </summary>
        BeltLook = 6,

        /// <summary>
        /// 客户成交
        /// </summary>
        EndDeal = 7,

        /// <summary>
        /// 激活客户
        /// </summary>
        Activation = 8,

        /// <summary>
        /// 失效客户
        /// </summary>
        Loss = 9,

        /// <summary>
        /// 加入公客池
        /// </summary>
        JoinPool = 10,

        /// <summary>
        /// 认领客户
        /// </summary>
        ClaimCustomer = 11,

        /// <summary>
        /// 认领客户
        /// </summary>
        TransferCustomer = 12
    }
    public enum NeedHouseType
    {
        HousingPrice = 1,
        HouseToBuy = 2,
        BusinessInquiry = 3,
        BusinessToBuy = 4
    }
}
