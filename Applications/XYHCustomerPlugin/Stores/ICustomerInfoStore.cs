using ApplicationCore.Dto;
using ApplicationCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    /// <summary>
    /// 接口
    /// </summary>
    public interface ICustomerInfoStore
    {
        IQueryable<CustomerInfo> CustomerInfos { get; set; }

        IQueryable<CustomerInfo> CustomerInfoSimple();

        IQueryable<CustomerInfo> CustomerInfoAll();

        IEnumerable<T> DapperSelect<T>(string sql);

        IQueryable<CustomerInfo> CustomerInfoFowllowUp();

        Task<List<T1>> RecommendFromBuilding<T1, T2>(string saleman, BuildingRecommendRequest condition, Func<T1, T2, T1> map);

        /// <summary>
        /// 查询部门详情
        /// </summary>
        /// <returns></returns>
        IQueryable<Organizations> OrganizationsAll();

        IQueryable<CustomerInfo> CustomerInfoInPoolAll();

        IQueryable<RelationHouse> CustomerInfoInRelationAll();

        IQueryable<CustomerInfo> CustomerInfoInLossAll();

        IQueryable<CustomerInfo> CustomerInfoInDealAll();

        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerInfo>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        Task<CustomerInfo> CreateAsync(CustomerInfo customerInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteAsync(UserInfo user, CustomerInfo customerInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task HandOverAsync(List<CustomerInfo> customerInfoList, List<CustomerDemand> customerdemandList, List<CustomerTransactions> customerTransactions, List<CustomerTransactionsFollowUp> customerTransactionsFollowUp, CancellationToken cancellationToken = default(CancellationToken));

        Task HandOverAsync(UserInfo user, List<CustomerInfo> customerInfoList, List<CustomerInfo> newcustomerInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task DeleteListAsync(List<CustomerInfo> customerInfoList, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateAsync(CustomerInfo customerInfo, CancellationToken cancellationToken = default(CancellationToken));

        Task UpdateListAsync(List<CustomerInfo> customerInfos, CancellationToken cancellationToken = default(CancellationToken));

        Task TransferringListAsync(List<CustomerInfo> customerInfos, List<CustomerFollowUp> customerFollowUps, List<CustomerFollowUp> newcustomerFollowUps, List<CustomerTransactions> customerTransactions, List<CustomerTransactionsFollowUp> customertranfus, List<BeltLook> beltLook, List<CustomerDemand> customerDemands, CancellationToken cancellationToken = default(CancellationToken));

        Task<ChekPhoneHistory> CreateCheckPhone(ChekPhoneHistory chekPhoneHistory, CancellationToken cancellationToken = default(CancellationToken));
    }
}
