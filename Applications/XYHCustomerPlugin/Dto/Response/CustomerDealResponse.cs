using System;
using System.Collections.Generic;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Response
{
    public class CustomerDealResponse
    {
        /// <summary>
        /// 销售方式
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 销售方式
        /// </summary>
        public SellerType SellerType { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 卖方
        /// </summary>
        public string Seller { get; set; }

        /// <summary>
        /// 报备流程ID
        /// </summary>
        public string FlowId { get; set; }

        /// <summary>
        /// 业务员
        /// </summary>
        public string Salesman { get; set; }

        /// <summary>
        /// 客户
        /// </summary>
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
        public string ProjectId { get; set; }

        /// <summary>
        /// 商铺ID
        /// </summary>
        public string ShopId { get; set; }

        /// <summary>
        /// 业主姓名
        /// </summary>
        public string Proprietor { get; set; }

        /// <summary>
        /// 业主电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 身份证号
        /// </summary>
        public string Idcard { get; set; }

        /// <summary>
        /// 居住地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 录入人
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        public bool IsDeleted { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string CustomerName { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        public string DepartmentName { get; set; }

        /// <summary>
        /// 客户电话
        /// </summary>
        public string CustomerPhone { get; set; }


        /// <summary>
        /// 楼盘名称
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// 商铺名称
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// 业务员姓名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 业务员电话
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }

        /// <summary>
        /// 审核状态
        /// </summary>
        public int? ExamineStatus { get; set; }

        public List<DealFileItemResponse> FileList { get; set; }

        public List<DealAttachmentResponse> AttachmentList { get; set; }
    }
}
