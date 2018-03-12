using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Dto
{
    public class CustomerInfoCreateResponse
    {
        public string Id { get; set; }
        
        public string CustomerName { get; set; }
        
        public string CustomerNo { get; set; }
        
        public string Source { get; set; }//来源 上门 报纸 网络 派单 端口   字典数据
        
        public string MainPhone { get; set; }//主用电话号码
        
        public string QQ { get; set; }
        
        public string WeChat { get; set; }
        
        public string Email { get; set; }

        public bool Sex { get; set; }

        public int Certificates { get; set; }//证件类型 "1 身份证2 军官证3 驾驶证4 护照5 其他"   字典数据
        
        public string CertificatesNo { get; set; }//证件号码

        public DateTime? Birthday { get; set; }

        public int Age { get; set; }//自动计算
        
        public string BrokerCondition { get; set; }//经济条件，一般经济，高新经济  字典数据

        public bool? LoanRecord { get; set; }//贷款记录

        public bool? Purchase { get; set; }//购房经历

        public int HouseNum { get; set; }//几套房

        public string Census { get; set; }//户籍
        
        public string CurrentAddress { get; set; }//现住址
        
        public string EngagedIndustry { get; set; }//从事行业  字典数据
        
        public string UnitAddress { get; set; }//单位地址
        
        public string FamilyDescription { get; set; }//家庭描述
        
        public DateTime? ConvenientHouseTime { get; set; }//方便看房时间   字典数据

        public DateTime? FollowupTime { get; set; }//跟进时间
        
        public string DepartmentId { get; set; }//部门
        
        public string UserId { get; set; }//员工

        public string UserName { get; set; }//员工

        public int CustomerStatus { get; set; }//客户状态  字典数据

        public int? ClassStatus { get; set; }

        public int BeltNum { get; set; }//带看次数

        public DateTime CreateTime { get; set; }

        public DateTime? FirstLookTime { get; set; }//首看时间

        public int? RateProgress { get; set; }//跟进阶段

        public NeedHouseType? HouseTypeId { get; set; }//需求类型    字典数据

        public int? ReferralType { get; set; }//转介类型

        public int? ReferralProportion { get; set; }//转化率

        public int? ReferralStatus { get; set; }//转介状态
        
        public string ReferralDepartmentId { get; set; }//转化部门
        
        public string ReferralUserId { get; set; }//转化人
        
        public string DistributionDepartmentId { get; set; }//分配部门

        public DateTime? TransferTime { get; set; }//转移日期

        public int ReferralCount { get; set; }//转化次数

        public bool? IsNewHourse { get; set; }//是否一手房

        public bool? IsComplexSee { get; set; }//是否看过房

        public int FollowUpNum { get; set; }//跟进数

        public int AboutNum { get; set; }//约看数

        public int NetiateNum { get; set; }
        
        public string ThreeNothing { get; set; }//三无情况
        
        public string DepartmentName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Mark { get; set; }

        public string HeadImg { get; set; }

        public CustomerLossResponse CustomerLossResponse { get; set; }

        public CustomerDemandResponse CustomerDemandResponse { get; set; }

        public IEnumerable<TransactionsFuResponse> TransactionsResponse { get; set; }

        //public IEnumerable<CustomerReportResponse> CustomerReportResponse { get; set; }

        public IEnumerable<BeltLookResponse> BeltLookResponse { get; set; }

        public IEnumerable<FollowUpResponse> CustomerFollowUpResponse { get; set; }

        public IEnumerable<RelationHouseResponse> RelationHouseResponse { get; set; }

        public IEnumerable<CustomerPhoneResponse> CustomerPhoneResponse { get; set; }

        public CustomerDealResponse CustomerDealResponse { get; set; }

        public List<DealFileItemResponse> FileList { get; set; }

    }
}
