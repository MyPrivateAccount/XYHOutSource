using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客户成交表
    /// </summary>
    public class CustomerDeal : TraceUpdateBase
    {
        /// <summary>
        /// 客户成交ID
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 销售方式
        /// </summary>
        [MaxLength(11)]
        public SellerType SellerType { get; set; }

        /// <summary>
        /// 卖方
        /// </summary>
        [MaxLength(255)]
        public string Seller { get; set; }

        /// <summary>
        /// 报备流程ID
        /// </summary>
        [MaxLength(127)]
        public string FlowId { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        [MaxLength(127)]
        public string Salesman { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        [MaxLength(127)]
        public string Customer { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        [MaxLength(18)]
        public decimal Commission { get; set; }

        /// <summary>
        /// 成交总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 楼盘ID
        /// </summary>
        [MaxLength(127)]
        public string ProjectId { get; set; }

        /// <summary>
        /// 商铺ID
        /// </summary>
        [MaxLength(127)]
        public string ShopId { get; set; }

        /// <summary>
        /// 业主姓名
        /// </summary>
        [MaxLength(255)]
        public string Proprietor { get; set; }

        /// <summary>
        /// 业主电话
        /// </summary>
        [MaxLength(255)]
        public string Mobile { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [MaxLength(255)]
        public string Idcard { get; set; }

        /// <summary>
        /// 居住地址
        /// </summary>
        [MaxLength(255)]
        public string Address { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(1000)]
        public string Mark { get; set; }

        /// <summary>
        /// 楼盘名称
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// 商铺名称
        /// </summary>
        public string ShopName { get; set; }

        public int? ExamineStatus { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        [NotMapped]
        public string CustomerName { get; set; }

        /// <summary>
        /// 客户姓名
        /// </summary>
        [NotMapped]
        public string CustomerPhone { get; set; }

        /// <summary>
        /// 业务员姓名
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }

        /// <summary>
        /// 业务员电话
        /// </summary>
        [NotMapped]
        public string UserPhone { get; set; }

        /// <summary>
        /// 部门Id
        /// </summary>
        [NotMapped]
        public string DepartmentId { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [NotMapped]
        public string DepartmentName { get; set; }

        /// <summary>
        /// 附件列表
        /// </summary>
        [NotMapped]
        public IEnumerable<CustomerDealFileInfo> DealFileInfos { get; set; }
    }

    public enum SellerType
    {
        /// <summary>
        /// 自售
        /// </summary>
        SinceSale = 1,

        /// <summary>
        /// 第三方销售
        /// </summary>
        ThirdPartySale = 2,

        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 10
    }
}
