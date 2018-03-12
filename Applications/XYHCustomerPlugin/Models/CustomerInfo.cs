using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using XYHCustomerPlugin.Dto.Request;

namespace XYHCustomerPlugin.Models
{
    /// <summary>
    /// 客户基础信息
    /// </summary>
    public class CustomerInfo : TraceUpdateBase
    {
        /// <summary>
        /// 主键GUID
        /// </summary>
        [Key]
        [MaxLength(127)]
        public string Id { get; set; }

        /// <summary>
        /// 客源唯一GUID
        /// </summary>
        [MaxLength(127)]
        public string UniqueId { get; set; }
        /// <summary>
        /// 客户姓名
        /// </summary>
        [MaxLength(50)]
        public string CustomerName { get; set; }
        /// <summary>
        /// 客户编号
        /// </summary>
        [MaxLength(50)]
        public string CustomerNo { get; set; }
        /// <summary>
        /// 字段数据，关联字典表存GUID，默认有：上门、报纸、网络、派单、端口
        /// </summary>
        [MaxLength(255)]
        public string Source { get; set; }
        /// <summary>
        /// 主用电话号码，加密存放
        /// </summary>
        [MaxLength(50)]
        public string MainPhone { get; set; }
        /// <summary>
        /// QQ
        /// </summary>
        [MaxLength(20)]
        public string QQ { get; set; }
        /// <summary>
        /// 微信
        /// </summary>
        [MaxLength(20)]
        public string WeChat { get; set; }
        /// <summary>
        /// 邮箱
        /// </summary>
        [MaxLength(50)]
        public string Email { get; set; }
        /// <summary>
        /// 性别（true为男，false为女）
        /// </summary>
        public bool Sex { get; set; }
        /// <summary>
        /// 证件类型，内部字典数据，1身份证2军官证3 驾驶证4 护照5 其他 
        /// </summary>
        public CustomerCertificates? Certificates { get; set; }
        /// <summary>
        /// 证件号码
        /// </summary>
        [MaxLength(50)]
        public string CertificatesNo { get; set; }
        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birthday { get; set; }
        /// <summary>
        /// 年龄-自动计算
        /// </summary>
        public int Age { get; set; }
        /// <summary>
        /// 经济条件，字典数据，存guid,默认有：一般经济，高新经济
        /// </summary>
        [MaxLength(255)]
        public string BrokerCondition { get; set; }
        /// <summary>
        /// 贷款记录
        /// </summary>
        public bool? LoanRecord { get; set; }
        /// <summary>
        /// 购房经历
        /// </summary>
        public bool? Purchase { get; set; }
        /// <summary>
        /// 几套房
        /// </summary>
        public int HouseNum { get; set; }
        /// <summary>
        /// 户籍
        /// </summary>
        [MaxLength(50)]
        public string Census { get; set; }
        /// <summary>
        /// 现住址
        /// </summary>
        [MaxLength(255)]
        public string CurrentAddress { get; set; }
        /// <summary>
        /// 从事行业，字典数据存GUID，默认有：娱乐行业、其他行业
        /// </summary>
        [MaxLength(255)]
        public string EngagedIndustry { get; set; }
        /// <summary>
        /// //单位地址
        /// </summary>
        [MaxLength(255)]
        public string UnitAddress { get; set; }
        /// <summary>
        /// //家庭描述
        /// </summary>
        [MaxLength(512)]
        public string FamilyDescription { get; set; }
        /// <summary>
        /// //方便看房时间，内部字典数据（1下班后 2 休息日 3随时）
        /// </summary>
        public DateTime? ConvenientHouseTime { get; set; }
        /// <summary>
        /// 最近一次跟进时间
        /// </summary>
        public DateTime? FollowupTime { get; set; }
        /// <summary>
        /// 所属部门
        /// </summary>
        [MaxLength(127)]
        public string DepartmentId { get; set; }
        /// <summary>
        /// 所属员工
        /// </summary>
        [MaxLength(127)]
        public string UserId { get; set; }
        /// <summary>
        /// //客户状态，内部字典数据（1 现有客源 2 不可用客源，包括失效包括已成交等等）
        /// </summary>
        public CustomerStatus? CustomerStatus { get; set; }
        /// <summary>
        /// 等级地位?（未确定字段，值有：0,1,2,3）暂不用
        /// </summary>
        public int? ClassStatus { get; set; }
        /// <summary>
        /// 带看次数
        /// </summary>
        public int BeltNum { get; set; }
        /// <summary>
        /// 首看时间
        /// </summary>
        public DateTime? FirstLookTime { get; set; }
        /// <summary>
        /// 商机阶段,内部字典数据（1录入2完善需求3约看（已报备） 4首看 5二看6诚意金 7洽谈 8成交 ）
        /// </summary>
        public RateProgress? RateProgress { get; set; }
        /// <summary>
        /// 房源类型，内部字典数据（1住宅求租2住宅求购 3商业求租4商业求购）
        /// </summary>
        public NeedHouseType? HouseTypeId { get; set; }
        /// <summary>
        /// 转移类型，内部字典数据（0手动转移1 未使用2 已使用 3离职自动转移）
        /// </summary>
        public ReferralType? ReferralType { get; set; }
        /// <summary>
        /// 转移比例，内部字典数据（0），未用
        /// </summary>
        public int? ReferralProportion { get; set; }
        /// <summary>
        /// 转移状态，内部字典数据（0执行成功 ?待转移  ?执行失败）
        /// </summary>
        public ReferralStatus? ReferralStatus { get; set; }
        /// <summary>
        /// 转移部门
        /// </summary>
        [MaxLength(127)]
        public string ReferralDepartmentId { get; set; }
        /// <summary>
        /// 转移操作人
        /// </summary>
        [MaxLength(127)]
        public string ReferralUserId { get; set; }
        /// <summary>
        /// 分配部门
        /// </summary>
        [MaxLength(127)]
        public string DistributionDepartmentId { get; set; }
        /// <summary>
        /// 转移日期
        /// </summary>
        public DateTime? TransferTime { get; set; }
        /// <summary>
        /// 转移次数
        /// </summary>
        public int ReferralCount { get; set; }
        /// <summary>
        /// 是否一手房
        /// </summary>
        public bool? IsNewHourse { get; set; }
        /// <summary>
        /// 是否看过房
        /// </summary>
        public bool? IsComplexSee { get; set; }
        /// <summary>
        /// 跟进次数
        /// </summary>
        public int FollowUpNum { get; set; }
        /// <summary>
        /// 约看次数
        /// </summary>
        public int AboutNum { get; set; }
        /// <summary>
        /// 谈判次数
        /// </summary>
        public int NetiateNum { get; set; }
        /// <summary>
        /// 三无情况，内部字典数据（0未填 1无本地户籍 2无本地企业 3无本地工作），可多选加逗号分隔
        /// </summary>
        [MaxLength(4000)]
        public string ThreeNothing { get; set; }
        /// <summary>
        /// 是否仍有购买意向
        /// </summary>
        public bool? IsSellIntention { get; set; }
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
        /// 头像
        /// </summary>
        public string HeadImg { get; set; }

        /// <summary>
        /// 源ID
        /// </summary>
        [MaxLength(127)]
        public string SourceId { get; set; }

        /// <summary>
        /// 业务员姓名
        /// </summary>
        [NotMapped]
        public string UserName { get; set; }

        /// <summary>
        /// 业务员工号
        /// </summary>
        [NotMapped]
        public string UserNumber { get; set; }
        /// <summary>
        /// 业务员电话
        /// </summary>
        [NotMapped]
        public string UserPhone { get; set; }

        /// <summary>
        /// 部门名称
        /// </summary>
        [NotMapped]
        public string DepartmentName { get; set; }

        /// <summary>
        /// 最近跟进房源
        /// </summary>
        [NotMapped]
        public string FollowUpHouses { get; set; }

        /// <summary>
        /// 需求信息
        /// </summary>
        [NotMapped]
        public CustomerDemand CustomerDemand { get; set; }

        /// <summary>
        /// 失效信息
        /// </summary>
        [NotMapped]
        public CustomerLoss CustomerLoss { get; set; }

        /// <summary>
        /// 成交信息
        /// </summary>
        [NotMapped]
        public IEnumerable<CustomerTransactions> CustomerTransactions { get; set; }

        /// <summary>
        /// 跟进情况
        /// </summary>
        [NotMapped]
        public IEnumerable<CustomerFollowUp> CustomerFollowUp { get; set; }

        /// <summary>
        /// 报备情况
        /// </summary>
        //[NotMapped]
        //public IEnumerable<CustomerReport> CustomerReport { get; set; }

        /// <summary>
        /// 带看情况
        /// </summary>
        [NotMapped]
        public IEnumerable<BeltLook> BeltLook { get; set; }

        /// <summary>
        /// 意向楼盘
        /// </summary>
        [NotMapped]
        public IEnumerable<RelationHouse> HousingResources { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        [NotMapped]
        public IEnumerable<CustomerPhone> CustomerPhones { get; set; }

        /// <summary>
        /// 电话2
        /// </summary>
        [NotMapped]
        public IEnumerable<CustomerPhone> CustomerPhones2 { get; set; }

        /// <summary>
        /// 公客池
        /// </summary>
        [NotMapped]
        public CustomerPool CustomerPool { get; set; }

        /// <summary>
        /// 成交信息
        /// </summary>
        [NotMapped]
        public CustomerDeal CustomerDeal { get; set; }

        [NotMapped]
        public IEnumerable<CustomerDealFileInfo> CustomerFileInfos { get; set; }
    }

    public enum CustomerCertificates
    {
        IdentificationCard = 1,
        ComeCard = 2,
        DriverCard = 3,
        Passport = 4,
        Others = 5
    }

    public enum ConvenientHouseTime
    {
        AfterWork = 1,
        DayOfRest = 2,
        AtAnyTime = 3
    }

    public enum CustomerStatus
    {
        /// <summary>
        /// 现有客户
        /// </summary>
        ExistingCustomers = 1,

        /// <summary>
        /// 不可用
        /// </summary>
        UnavailableSource = 2,

        /// <summary>
        /// 无效
        /// </summary>
        Lapse = 3,

        /// <summary>
        /// 已成交
        /// </summary>
        EndDeal = 4,

        /// <summary>
        /// 公客池客户
        /// </summary>
        PoolCustomer = 5,
    }

    public enum RateProgress
    {
        /// <summary>
        /// 录入
        /// </summary>
        Entry = 1,

        /// <summary>
        /// 完善需求
        /// </summary>
        PerfectDemand = 2,

        /// <summary>
        /// 约看
        /// </summary>
        TakeALookAt = 3,

        /// <summary>
        /// 首看
        /// </summary>
        Look = 4,

        /// <summary>
        /// 二看
        /// </summary>
        SencondLook = 5,

        /// <summary>
        /// 诚意金
        /// </summary>
        SincerityGold = 6,

        /// <summary>
        /// 洽谈
        /// </summary>
        Negotiation = 7,

        /// <summary>
        /// 成交
        /// </summary>
        ClinchADeal = 8
    }

    public enum ReferralType
    {
        ManualTransfer = 0,
        Unused = 1,
        HaveBeenUsed = 2,
        AutomaticTransfer = 3
    }

    public enum ReferralStatus
    {
        ExecutionSuccess = 0,
        Totransfer = 1,
        Onfailure = 2
    }


}
