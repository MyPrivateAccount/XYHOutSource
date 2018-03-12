using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Request
{
    /// <summary>
    /// 成交信息请求体
    /// </summary>
    public class TransactionsRequest
    {
        /// <summary>
        /// 合同ID
        /// </summary>
        public string ContractId { get; set; }
        /// <summary>
        /// 合同编号
        /// </summary>
        public string ContractNo { get; set; }
        /// <summary>
        /// 客户Id
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        public string CustomerNo { get; set; }
        /// <summary>
        /// 房源Id
        /// </summary>
        public string HousingResourcesId { get; set; }
        /// <summary>
        /// 房源编号
        /// </summary>
        public string HousingResourcesNo { get; set; }
        /// <summary>
        /// 签约时间
        /// </summary>
        public string SignTime { get; set; }
        /// <summary>
        /// 商圈Id
        /// </summary>
        public string DistrictId { get; set; }
        /// <summary>
        /// 区域Id
        /// </summary>
        public string AreaId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PropertyId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PropertyAddress { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户类型
        /// </summary>
        public int? CustomerType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? TurnoverAmount { get; set; }
        /// <summary>
        /// 成交类型
        /// </summary>
        public int? TransactionType { get; set; }
        /// <summary>
        /// 价格
        /// </summary>
        public decimal? HousingPrices { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? Commission { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 部门Id
        /// </summary>
        public string DepartmentId { get; set; }
        /// <summary>
        /// 主要电话号码
        /// </summary>
        public string MainPhone { get; set; }
        /// <summary>
        /// 价格改变状态
        /// </summary>
        public int? PriceChangeState { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public decimal? DifferencePrices { get; set; }
        /// <summary>
        /// 跟进时间
        /// </summary>
        public DateTime? FollowUpTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 需求等级
        /// </summary>
        public string DemandLevel { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string KeyTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SurveyTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FullTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsPicture { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PropertyUse { get; set; }
        /// <summary>
        /// 房源面积
        /// </summary>
        public decimal? HousingArea { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ApartmentLayout { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool? IsVisit { get; set; }
        
        
    }
}
