using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Request
{
    public class CustomerDealRequest
    {
        /// <summary>
        /// 销售方式
        /// </summary>
        public SellerType SellerType { get; set; }

        /// <summary>
        /// 卖方
        /// </summary>
        [StringLength(255)]
        public string Seller { get; set; }

        /// <summary>
        /// 报备流程ID
        /// </summary>
        [StringLength(127)]
        public string FlowId { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        [StringLength(127)]
        public string Salesman { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
        [StringLength(127)]
        public string Customer { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 成交总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 楼盘ID
        /// </summary>
        [StringLength(127)]
        public string ProjectId { get; set; }

        /// <summary>
        /// 商铺ID
        /// </summary>
        [StringLength(127)]
        public string ShopId { get; set; }

        /// <summary>
        /// 业主姓名
        /// </summary>
        [StringLength(255)]
        public string Proprietor { get; set; }

        /// <summary>
        /// 业主电话
        /// </summary>
        [StringLength(255)]
        public string Mobile { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        [StringLength(255)]
        public string Idcard { get; set; }

        /// <summary>
        /// 居住地址
        /// </summary>
        [StringLength(255)]
        public string Address { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [StringLength(1000)]
        public string Mark { get; set; }

        /// <summary>
        /// 是否二手交易
        /// </summary>
        public bool IsTwoHand { get; set; }

        /// <summary>
        /// 楼盘名称
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// 商铺名称
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// 文件列表
        /// </summary>
        public List<DealFileInfoRequest> FileList { get; set; }
    }
}
