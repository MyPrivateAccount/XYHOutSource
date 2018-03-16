using System;
using System.Collections.Generic;
using System.Text;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto.Response
{
    /// <summary>
    /// 成交信息返回体
    /// </summary>
    public class TransactionsResponse
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
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
        /// 区-部门-组
        /// </summary>
        public string DepartmentName { get; set; }
        /// <summary>
        /// 主要电话号码
        /// </summary>
        public string MainPhone { get; set; }

        /// <summary>
        /// 加密电话集
        /// </summary>
        public List<string> Phones { get; set; }

        /// <summary>
        /// 主要电话号码
        /// </summary>
        public string TruePhone { get; set; }

        /// <summary>
        /// 未加密
        /// </summary>
        public List<string> TruePhones { get; set; }
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
        public bool IsDeleted { get; set; }
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


        /// <summary>
        /// 跟进信息
        /// </summary>
        public List<TransactionsFollowUpResponse> TransactionsFollowUpResponse { get; set; }

        //以下为新增信息

        /// <summary>
        /// 业务员手机号
        /// </summary>
        public string UserPhone { get; set; }

        /// <summary>
        /// 业务员真实姓名
        /// </summary>
        public string UserTrueName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public TransactionsStatus TransactionsStatus { get; set; }

        /// <summary>
        /// 带看人Id
        /// </summary>
        public string BeltLookId { get; set; }

        /// <summary>
        /// 带看时间
        /// </summary>
        public DateTime? BeltLookTime { get; set; }

        /// <summary>
        /// 报备时间
        /// </summary>
        public DateTime ReportTime { get; set; }

        /// <summary>
        /// 报备楼盘
        /// </summary>
        public string BuildingId { get; set; }

        /// <summary>
        /// 报备商铺
        /// </summary>
        public string ShopsId { get; set; }

        /// <summary>
        /// 楼栋名称
        /// </summary>
        public string BuildingName { get; set; }

        /// <summary>
        /// 商铺名称
        /// </summary>
        public string ShopsName { get; set; }

        /// <summary>
        /// 预计带看时间
        /// </summary>
        public DateTime? ExpectedBeltTime { get; set; }

        public bool? IsSellIntention { get; set; }
    }
}
