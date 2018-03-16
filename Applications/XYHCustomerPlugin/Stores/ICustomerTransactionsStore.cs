using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public interface ICustomerTransactionsStore
    {
        IQueryable<CustomerTransactions> CustomerTransactions { get; set; }

        /// <summary>
        /// 查询成交信息详情
        /// </summary>
        /// <returns></returns>
        IQueryable<CustomerTransactions> CustomerTransactionsAll();

        /// <summary>
        /// 根据某一字段获取一条成交信息
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerTransactions>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 根据条件获取集合
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="query"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerTransactions>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 创建成交信息
        /// </summary>
        /// <param name="customertransactions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<CustomerTransactions> CreateAsync(CustomerTransactions customertransactions, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 创建成交信息v2
        /// </summary>
        /// <param name="customerfollowups"></param>
        /// <param name="customerInfo"></param>
        /// <param name="customerTransactions"></param>
        /// <param name="customerTransacitonsFollowUps"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task CreateAndUpdateUserInfoAsync(List<CustomerFollowUp> customerfollowups, List<CustomerInfo> customerInfo, List<CustomerTransactions> customerTransactions, List<CustomerTransactions> customerTransactionsup, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 删除客户成交
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="customertransactions">客户成交实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        Task DeleteAsync(SimpleUser user, CustomerTransactions customertransactions, CancellationToken cancellationToken = default(CancellationToken));


        /// <summary>
        /// 从数据库中删除List
        /// </summary>
        /// <param name="customertransactionsList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        Task DeleteListAsync(List<CustomerTransactions> customertransactionsList, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 修改客户成交信息
        /// </summary>
        /// <param name="customertransactions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(CustomerTransactions customertransactions, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 修改客户成交信息并增加跟进
        /// </summary>
        /// <param name="customertransactions"></param>
        /// <param name="customerTransactionsFollowUp"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateAsync(CustomerTransactions customertransactions, CustomerTransactionsFollowUp customerTransactionsFollowUp, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// 批量修改客户成交信息(一般修改删除状态)
        /// </summary>
        /// <param name="customertransactionss"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task UpdateListAsync(List<CustomerTransactions> customertransactionss, CancellationToken cancellationToken = default(CancellationToken));
    }
}
